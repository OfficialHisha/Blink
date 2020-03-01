using System;
using System.Collections.Generic;
using System.Text;

namespace DM_Spiljam_Server
{
    class Entity
    {
        private static int nextEntityId = 0;
        public static int NextEntityId => nextEntityId++;

        public int EntityId { get; }
        public string Owner { get; }
        public float X { get; private set; }
        public float Y { get; private set; }

        public Entity(int entityId, string owner, float x = 0, float y = 0)
        {
            EntityId = entityId;
            Owner = owner;
            X = x;
            Y = y;
        }

        public void UpdatePosition(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}
