﻿using System;
using UnityEngine;

namespace DeviceSyncUnity.Messages
{
    public class AccelerationMessage : DevicesSyncMessage
    {
        // Properties

        public override int SenderConnectionId { get { return senderConnectionId; } set { senderConnectionId = value; } }

        // Variables

        public int senderConnectionId;
        public Vector3 acceleration;
        public float deltaTime;
        public AccelerationEventMessage[] accelerationEvents;

        // Methods

        public void UpdateInfo()
        {
            acceleration += Input.acceleration;
            deltaTime += Time.deltaTime;

            int previousLength = 0;
            if (accelerationEvents == null)
            {
                accelerationEvents = new AccelerationEventMessage[Input.accelerationEventCount];
            }
            else
            {
                previousLength = accelerationEvents.Length;
                Array.Resize(ref accelerationEvents, accelerationEvents.Length + Input.accelerationEventCount);
            }

            int i = 0;
            while (i < Input.accelerationEventCount)
            {
                // TODO: check if the order of stacked events if correct
                accelerationEvents[i + previousLength] = Input.GetAccelerationEvent(i);
                i++;
            }
        }
    }
}
