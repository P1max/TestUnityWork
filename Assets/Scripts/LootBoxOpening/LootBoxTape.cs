using AxGrid.Base;
using AxGrid.Path;
using System.Collections.Generic;
using UnityEngine;

namespace LootBoxOpening
{
    public class LootBoxTape : MonoBehaviourExtBind
    {
        private readonly List<RectTransform> _items = new();

        [Header("Настройки скролла")]
        [SerializeField] private float _maxSpeed = 2500f;
        [SerializeField, Tooltip("Как быстро лента набирает максимальную скорость")]
        private float _acceleration = 500f;

        [Header("Настройки остановки")]
        [SerializeField, Tooltip("Сколько дополнительных кругов сделает рулетка перед остановкой")]
        private int _extraSpins = 3;

        [Header("Ссылки")]
        [SerializeField] private RectTransform _container;
        [SerializeField] private ParticleSystem _winParticles;

        private CPath _animationPath;
        private float _scrollPosition;
        private float _visualOffsetY;
        private float _itemHeight;
        private float _currentSpeed;
        private float _totalHeight;
        private bool _isScrolling;

        [OnAwake]
        private void Init()
        {
            Model.EventManager.AddAction("StartVisualScroll", StartScroll);
            Model.EventManager.AddAction("StopVisualScroll", StopScroll);

            foreach (RectTransform child in _container)
                _items.Add(child);

            if (_items.Count > 0)
            {
                var layout = _container.GetComponent<UnityEngine.UI.VerticalLayoutGroup>();

                if (layout != null)
                {
                    UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(_container);

                    var elementHeight = _items[0].rect.height;
                    var spacing = layout.spacing;

                    _itemHeight = elementHeight + spacing;
                    _visualOffsetY = _items[0].anchoredPosition.y;

                    layout.enabled = false;
                }
                else Debug.LogWarning("VerticalLayoutGroup не найден.");
            }

            _totalHeight = _items.Count * _itemHeight;

            UpdatePositions();
        }

        [OnUpdate]
        private void OnUpdate()
        {
            _animationPath?.Update(Time.deltaTime);

            if (!_isScrolling) return;

            _currentSpeed = Mathf.MoveTowards(_currentSpeed, _maxSpeed, Time.deltaTime * _acceleration);
            _scrollPosition += _currentSpeed * Time.deltaTime;

            UpdatePositions();
        }

        private void UpdatePositions()
        {
            for (var i = 0; i < _items.Count; i++)
            {
                var itemOffset = i * _itemHeight;
                var loopedPos = Mathf.Repeat(_scrollPosition + itemOffset, _totalHeight);
                var finalY = -loopedPos + _visualOffsetY;

                _items[i].anchoredPosition = new Vector2(_items[i].anchoredPosition.x, finalY);
            }
        }

        private void StartScroll()
        {
            _isScrolling = true;
            _animationPath = null;
            _currentSpeed = 0;
            _scrollPosition %= _totalHeight;

            if (_scrollPosition < 0) _scrollPosition += _totalHeight;
        }

        private void StopScroll()
        {
            _isScrolling = false;

            var itemsPassed = _scrollPosition / _itemHeight;
            var targetIndex = Mathf.Ceil(itemsPassed) + (_items.Count * _extraSpins);
            var targetPos = targetIndex * _itemHeight;
            var distance = targetPos - _scrollPosition;
            var startSpeed = Mathf.Max(_currentSpeed, 10f);
            var duration = (3f * distance) / startSpeed;

            _animationPath = new CPath();
            _animationPath
                .EasingCubicEaseOut(duration, _scrollPosition, targetPos, (val) =>
                {
                    _scrollPosition = val;

                    UpdatePositions();
                })
                .Action(() =>
                {
                    _scrollPosition = targetPos;

                    UpdatePositions();

                    _winParticles?.Play();
                    Model.EventManager.Invoke("OnAnimationFinished");
                });
        }
    }
}