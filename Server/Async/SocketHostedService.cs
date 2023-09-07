using Microsoft.Extensions.Configuration;
using Server.Dtos.Position;
using Server.Infra;
using Server.Repositories;
using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Server.Entities;

namespace Core.Async
{
    public class SocketHostedService : IDisposable
    {
        private SocketHandler io;
        private UserRepository userRepository;
        public SocketHostedService(SurvivalContext ctx,IConfiguration conf)
        {
            userRepository = new UserRepository(ctx);
            io = new SocketHandler(conf);
            io.WatcherHealth();
            io.On("player_log", OnPlayerLog);
            io.On("update_position", OnUpdatePosition);
        }

        private void OnUpdatePosition(IPEndPoint ip, string data)
        {
            Console.WriteLine($"{ip} move to {data}");
           var pos= JsonConvert.DeserializeObject<UpdatePosition>(data);
           if(io.players.TryGetValue(ip,out UserEntity user))
            {
                user.LasPosition = (PositionEntity)pos;
                user.LasPosition.Id = user.LasPositionId;
                io.players.AddOrUpdate(ip, user, (a, b) => {
                   
                    b.LasPosition = (PositionEntity)pos;
                    b.LasPosition.Id = b.LasPositionId;

                    return b;
                });
                Console.WriteLine(pos);

            }
        }


        private void OnPlayerLog(IPEndPoint ip, string id)
        {

            Console.WriteLine(id);
            var user = userRepository.GetById(Guid.Parse(id));
            Console.WriteLine(user);
            if (user == null) return;
            io.players.AddOrUpdate(ip,user,(a,b)=> {
                return user;
            } );
            io.Emit(ip, "player_log_cb", user.toJson());

        }

        public void Dispose()
        {
            io.Dispose();
        }

        public Task StartAsync()
        {

            Console.WriteLine("Server Starting");
            Task.Delay(10 * 100).ContinueWith(task =>
            {

                io.Start(12345);
                io.On("chat", (ip, data) =>
                {
                    Console.WriteLine(data);
                    io.Emit(ip, "chat_feedback", "received");
                });
            });

            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            io.Dispose();
            return Task.CompletedTask;

        }
    }
}
