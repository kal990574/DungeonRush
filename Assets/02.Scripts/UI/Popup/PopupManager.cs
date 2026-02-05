using System;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonRush.UI.Popup
{
    public class PopupManager : MonoBehaviour
    {
        [Header("Popup Views")]
        [SerializeField] private CardSelectView _cardSelectView;
        [SerializeField] private GameOverView _gameOverView;

        private readonly List<IDisposable> _presenters = new List<IDisposable>();

        private void Start()
        {
            _cardSelectView.gameObject.SetActive(false);
            _gameOverView.gameObject.SetActive(false);

            CreatePresenters();
        }

        private void OnDestroy()
        {
            DisposePresenters();
        }

        private void CreatePresenters()
        {
            var cardSelectPresenter = new CardSelectPresenter(
                new CardSelectModel(), _cardSelectView);
            cardSelectPresenter.Enable();
            _presenters.Add(cardSelectPresenter);

            var gameOverPresenter = new GameOverPresenter(
                new GameOverModel(), _gameOverView);
            gameOverPresenter.Enable();
            _presenters.Add(gameOverPresenter);
        }

        private void DisposePresenters()
        {
            foreach (IDisposable presenter in _presenters)
            {
                presenter.Dispose();
            }

            _presenters.Clear();
        }
    }
}
