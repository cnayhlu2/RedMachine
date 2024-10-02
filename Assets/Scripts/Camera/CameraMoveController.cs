using Player.ActionHandlers;
using UnityEngine;
using Events;
using Vector3 = UnityEngine.Vector3;
using System.Collections;
using Assets.Scripts.Camera;

namespace Camera
{
    public class CameraMoveController : MonoBehaviour
    {
        private const string TAG = nameof(CameraMoveController);
        [SerializeField] private CameraMoveConfig _config;

        private Transform _moveTarget;
        private ClickHandler _clickHandler;
        private Vector3 _dragStartPosition;
        private Vector3 _movePosition;
        private bool _canDrag;
        private bool _isDraging;
        Coroutine _coroutine;

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

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            vector.z = 0;
            _movePosition = _moveTarget.position + (_dragStartPosition - vector);
            _coroutine = StartCoroutine(MoveCoroutine());
        }

        private void DragStartEvent(Vector3 vector)
        {
            if (!_canDrag)
            {
                return;
            }
            _isDraging = true;
            vector.z = 0;
            _dragStartPosition = vector;
        }


        private IEnumerator MoveCoroutine()
        {
            while (Vector3.SqrMagnitude(_moveTarget.position - _movePosition) > _config.StoppingDistance)
            {
                Vector3 direction = (_moveTarget.position - _movePosition).normalized;
                _moveTarget.position -= direction * Time.deltaTime * _config.MoveSpeed;
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
    }
}