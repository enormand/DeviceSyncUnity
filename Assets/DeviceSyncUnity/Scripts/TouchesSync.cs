﻿using DeviceSyncUnity.Messages;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DeviceSyncUnity
{
    // TODO: add a visual debugger that display the touches info on screen
    public class TouchesSync : DevicesSync
    {
        // Editor fields

        [SerializeField]
        private SendingMode sendingMode = SendingMode.TimeInterval;

        [SerializeField]
        private float sendingTimeInterval = 0.1f;

        [SerializeField]
        private uint sendingFramesInterval = 2;

        // Properties

        public override SendingMode SendingMode { get { return sendingMode; } set { sendingMode = value; } }
        public override float SendingTimeInterval { get { return sendingTimeInterval; } set { sendingTimeInterval = value; } }
        public override uint SendingFramesInterval { get { return sendingFramesInterval; } set { sendingFramesInterval = value; } }

        public Dictionary<int, TouchesMessage> LastTouchesReceived { get; protected set; }

        protected override short MessageType { get { return Messages.MessageType.Touches; } }

        // Variables

        protected Stack<TouchMessage[]> previousTouches = new Stack<TouchMessage[]>();

        // Events

        public event Action<TouchesMessage> ServerTouchesReceived = delegate { };
        public event Action<TouchesMessage> ClientTouchesReceived = delegate { };

        // Methods

        protected virtual void Awake()
        {
            LastTouchesReceived = new Dictionary<int, TouchesMessage>();
        }

        protected override void OnSendToServerIntervalIteration(bool send)
        {
            var touchesMessage = new TouchesMessage();
            touchesMessage.Populate(Camera.main);

            if (!send)
            {
                previousTouches.Push(touchesMessage.touches);
            }
            else
            {
                touchesMessage.SetTouchesAverage(previousTouches);
                previousTouches.Clear();
                SendToServer(touchesMessage);
            }
        }

        protected override DevicesSyncMessage OnSendToAllClientsInternal(NetworkMessage netMessage)
        {
            var touchesMessage = netMessage.ReadMessage<TouchesMessage>();
            ServerTouchesReceived.Invoke(touchesMessage);
            return touchesMessage;
        }

        protected override DevicesSyncMessage OnClientReceiveInternal(NetworkMessage netMessage)
        {
            var touchesMessage = netMessage.ReadMessage<TouchesMessage>();
            LastTouchesReceived[touchesMessage.senderConnectionId] = touchesMessage;
            ClientTouchesReceived.Invoke(touchesMessage);
            return touchesMessage;
        }
    }
}