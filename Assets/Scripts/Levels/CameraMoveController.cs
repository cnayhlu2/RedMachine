using Player.ActionHandlers;
using UnityEngine;
using Events;
using Vector3 = UnityEngine.Vector3;
using Camera;

namespace Levels
{
    public class CameraMoveController : MonoBehaviour
    {
        private const string TAG = nameof(CameraMoveController);
        private Transform _moveTarget;
        private ClickHandler _clickHandler;
        private Vector3 _dragPosition;
        private bool _canDrag;
        private bool _isDraging;

        private void Start()
        {
            _clickHandler = ClickHandler.Instance;
            _clickHandler.DragStartEvent += DragStartEvent;
            _clickHandler.DragEndEvent += DragEndEvent;
            _moveTarget = CameraHolder.Instance.MainCamera.transform;

            EventsController.Subscribe<EventModels.Game.NodeTapped>(this, OnNodeTapped);
            EventsController.Subscribe<EventModels.Game.PlayerFingerRemoved>(this, OnPlayerFingerRemoved);
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            _clickHandler.DragStartEvent -= DragStartEvent;
            _clickHandler.DragEndEvent -= DragEndEvent;
            EventsController.Unsubscribe<EventModels.Game.NodeTapped>(OnNodeTapped);
            EventsController.Unsubscribe<EventModels.Game.PlayerFingerRemoved>(OnPlayerFingerRemoved);
        }

        private void OnPlayerFingerRemoved(EventModels.Game.PlayerFingerRemoved removed)
        {
            _canDrag = true;
        }

        private void OnNodeTapped(EventModels.Game.NodeTapped tapped)
        {
            _canDrag = false;
        }

        private void DragEndEvent(Vector3 vector)
        {
            _isDraging = false;
            if (!_canDrag)
            {
                return;
            }

            Vector3 direction = vector - _dragPosition;
            direction.z = 0;
            _moveTarget.position -= direction;
        }

        private void DragStartEvent(Vector3 vector)
        {
            if (!_canDrag)
            {
                return;
            }
            _isDraging = true;
            _dragPosition = vector;
        }

    }
}