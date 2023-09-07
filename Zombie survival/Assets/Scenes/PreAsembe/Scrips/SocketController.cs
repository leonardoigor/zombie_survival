using Assets.Utils.Extensions;
using System;
using UnityEditor;
using UnityEngine;

public class SocketController : MonoBehaviour
{

    private ClientWrapper io;
    [SerializeField] private int port = 12345;
    [SerializeField] private string host = "127.0.0.0";
    [SerializeField] public Guid Id;
    public GameObject Player;
    public Vector3 lastPosition;
    [SerializeField]
    public Vector3 CurrentPosition
    {
        get
        {
            return Player.transform.localPosition;
        }
    }

    void Start()
    {
        lastPosition = Player.transform.localPosition;
        EditorApplication.playModeStateChanged += OnChangePlayerMode;
        Id = Guid.NewGuid();
        Debug.Log(Id);
        io = new ClientWrapper(host, port, Id.ToString());
        StartCoroutine(io.HealthCheck());
        io.On("player_log_cb", OnPlayerLogCb);
        io.Send("player_log", "C2556F6D-6E5A-4686-BEA3-9F6E9D339E04");

    }

    private void OnChangePlayerMode(PlayModeStateChange obj)
    {
        if (!EditorApplication.isPlayingOrWillChangePlaymode && EditorApplication.isPlaying)
        {
            io.Close();
        }
    }

    private void OnPlayerLogCb(string data)
    {
        Debug.Log(data);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
        }
    }
    void FixedUpdate()
    {
        var cond = Vector3.Distance(lastPosition, CurrentPosition) > 0.01;
        if (cond)
        {
            var p = CurrentPosition;
            var r = Player.transform.localRotation;
            lastPosition = Player.transform.localPosition;
            string pos = p.toJson(r);
            io.Send("update_position", pos);
        }

    }

}
