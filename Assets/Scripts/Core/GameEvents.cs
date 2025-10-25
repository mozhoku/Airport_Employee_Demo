using Devices;
using NPC;
using UnityEngine;

namespace Core
{
    public static class GameEvents
    {
        // General Game Events
        public struct GameStarted
        {
        }

        public struct GameCompleted
        {
        }

        public struct PlayerMovementEnabled
        {
            public bool Enabled;
            public PlayerMovementEnabled(bool enabled) => Enabled = enabled;
        }

        public struct ReceptionCompleted
        {
        }

        public struct PaintingStarted
        {
        }

        public struct PaintingCompleted
        {
        }

        public struct AreaUnlockStarted
        {
        }

        public struct AreaUnlockCompleted
        {
            public bool IsCompleted;

            public AreaUnlockCompleted(bool isCompleted)
            {
                IsCompleted = isCompleted;
            }
        }

        // Camera Events
        public struct CameraPanToAreaComplete
        {
        }

        public struct CameraPanBackComplete
        {
        }

        // Luggage Events
        public struct LuggageGiven
        {
            public Transform Luggage;
            public NPCController NpcController;

            public LuggageGiven(Transform luggage, NPCController npcController)
            {
                Luggage = luggage;
                NpcController = npcController;
            }
        }

        public struct LuggageTaken
        {
            public Transform Npc;
            public NPCController Controller;

            public LuggageTaken(Transform npc, NPCController controller)
            {
                Npc = npc;
                Controller = controller;
            }
        }

        public struct LuggageDropped
        {
            public Transform Luggage;
            public LuggageDropped(Transform luggage) => Luggage = luggage;
        }

        public struct LuggagePushed
        {
            public Transform Luggage;
            public LuggagePushed(Transform luggage) => Luggage = luggage;
        }

        public struct AllLuggagesScanned
        {
        }


        // Area enter events
        public struct AreaUnlockEntered
        {
            public bool PlayerEntered;

            public AreaUnlockEntered(bool playerEntered)
            {
                PlayerEntered = playerEntered;
            }
        }

        public struct AreaUnlockExited
        {
            public bool PlayerExited;

            public AreaUnlockExited(bool playerExited)
            {
                PlayerExited = playerExited;
            }
        }

        public struct ReceptionEntered
        {
            public bool PlayerEntered;
            public Transform TargetTransform;

            public ReceptionEntered(bool playerEntered, Transform targetTransform)
            {
                PlayerEntered = playerEntered;
                TargetTransform = targetTransform;
            }
        }

        public struct ReceptionExited
        {
            public bool PlayerExited;

            public ReceptionExited(bool playerExited)
            {
                PlayerExited = playerExited;
            }
        }

        public struct LuggageDropAreaEntered
        {
            public bool PlayerEntered;

            public LuggageDropAreaEntered(bool playerEntered)
            {
                PlayerEntered = playerEntered;
            }
        }

        public struct LuggageDropAreaExited
        {
            public bool PlayerExited;

            public LuggageDropAreaExited(bool playerExited)
            {
                PlayerExited = playerExited;
            }
        }

        public struct XRay1AreaEntered
        {
            public bool PlayerEntered;

            public XRay1AreaEntered(bool playerEntered)
            {
                PlayerEntered = playerEntered;
            }
        }

        public struct XRay1AreaExited
        {
            public bool PlayerExited;

            public XRay1AreaExited(bool playerExited)
            {
                PlayerExited = playerExited;
            }
        }

        public struct XRay2AreaEntered
        {
            public bool PlayerEntered;

            public XRay2AreaEntered(bool playerEntered)
            {
                PlayerEntered = playerEntered;
            }
        }

        public struct XRay2AreaExited
        {
            public bool PlayerExited;

            public XRay2AreaExited(bool playerExited)
            {
                PlayerExited = playerExited;
            }
        }

        public struct PaintingAreaEntered
        {
            public bool PlayerEntered;

            public PaintingAreaEntered(bool playerEntered)
            {
                PlayerEntered = playerEntered;
            }
        }

        public struct NPCReachedCheckpoint
        {
            public NPCController Controller;
            public NPCWaypoint Waypoint;

            public NPCReachedCheckpoint(NPCController controller, NPCWaypoint waypoint)
            {
                Controller = controller;
                Waypoint = waypoint;
            }
        }

        public struct NPCWaiting
        {
            public NPC.NPCController Controller;

            public NPCWaiting(NPC.NPCController c)
            {
                Controller = c;
            }
        }

        public struct NPCResume
        {
            public NPC.NPCController Controller;

            public NPCResume(NPC.NPCController c)
            {
                Controller = c;
            }
        }

        // Elevator Events
        public struct ElevatorRideStarted
        {
            public Transform Rider;
            public Elevator Elevator;

            public ElevatorRideStarted(Transform rider, Elevator elevator)
            {
                Rider = rider;
                Elevator = elevator;
            }
        }

        public struct ElevatorRideUpdated
        {
            public Transform Rider;
            public Elevator Elevator;
            public Vector3 DeltaMotion;
            public int CurrentFloor;


            public ElevatorRideUpdated(Transform rider, Elevator elevator, Vector3 delta, int currentFloor)
            {
                Rider = rider;
                Elevator = elevator;
                DeltaMotion = delta;
                CurrentFloor = currentFloor;
            }
        }

        public struct ElevatorRideEnded
        {
            public Transform Rider;
            public Elevator Elevator;
            public int CurrentFloor;

            public ElevatorRideEnded(Transform rider, Elevator elevator, int currentFloor)
            {
                Rider = rider;
                Elevator = elevator;
                CurrentFloor = currentFloor;
            }
        }
    }
}