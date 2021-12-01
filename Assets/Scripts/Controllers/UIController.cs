using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
// Handles displaying game state
public class UIController : MonoBehaviour
{
    [TitleGroup("Component References"), SerializeField] GameObject _cardPrefab;
    [TitleGroup("Component References"), SerializeField] GameController _gameController;

    [BoxGroup("Sidebar")]
    [SerializeField, BoxGroup("Sidebar")] TextMeshProUGUI _roundCount;
    [SerializeField, BoxGroup("Sidebar/ID1")] TextMeshProUGUI _index1Name;
    [SerializeField, BoxGroup("Sidebar/ID2")] TextMeshProUGUI _index2Name;
    [SerializeField, BoxGroup("Sidebar/ID1")] TextMeshProUGUI _index1DeckCount;
    [SerializeField, BoxGroup("Sidebar/ID2")] TextMeshProUGUI _index2DeckCount;
    [SerializeField, BoxGroup("Sidebar/ID1")] TextMeshProUGUI _index1HP;
    [SerializeField, BoxGroup("Sidebar/ID2")] TextMeshProUGUI _index2HP;
    [SerializeField, BoxGroup("Sidebar/ID1")] Image _index1Icon;
    [SerializeField, BoxGroup("Sidebar/ID2")] Image _index2Icon;

    [SerializeField, BoxGroup("Board")] Transform _index1CardContainer;
    [SerializeField, BoxGroup("Board")] Transform _index2CardContainer;
    [SerializeField, BoxGroup("Board")] Transform _deck1Transform;
    [SerializeField, BoxGroup("Board")] Transform _deck2Transform;
    [SerializeField, BoxGroup("Board")] Transform _board;


    [SerializeField, BoxGroup("Board")] List<CardController> _player1Cards = new List<CardController>();
    [SerializeField, BoxGroup("Board")] List<CardController> _player2Cards = new List<CardController>();




    Player _player1;
    Player _player2;


    public void InitialiseGame(Player player1, Player player2)
    {
        _player1 = player1;
        _player2 = player2;

        _index1Name.text = player1.PlayerPepemon.DisplayName;
        _index2Name.text = player2.PlayerPepemon.DisplayName;

        _index1DeckCount.text = player1.PlayerDeck.GetDeck().Count + "cards";
        _index2DeckCount.text = player2.PlayerDeck.GetDeck().Count + "cards";

        _index1HP.text = player1.CurrentHP + "hp";
        _index2HP.text = player2.CurrentHP + "hp";

        _index1Icon.sprite = player1.PlayerPepemon.DisplayIcon;
        _index2Icon.sprite = player2.PlayerPepemon.DisplayIcon;
    }


    // part1 lay down all AD + DF cards
    // play the hand
    // Reverse role
    // next round

    public void DisplayHands()
    {
        // Cleanup old hand
        foreach (Transform child in _index1CardContainer) GameObject.Destroy(child.gameObject);
        foreach (Transform child in _index2CardContainer) GameObject.Destroy(child.gameObject);

        // Display new hand
        StartCoroutine(DrawCards(_player1));
        StartCoroutine(DrawCards(_player2));

        // for (int i = 0; i < _player2.CurrentHand.GetCardsInHand.Count; i++)
        // {
        //     GameObject go = Instantiate(_cardPrefab);
        //     go.transform.SetParent(_index2CardContainer);
        //     go.GetComponent<CardController>().PouplateCard(_player2.CurrentHand.GetCardsInHand[i]);
        // }

        // Update deck count
        _index1DeckCount.text = _player1.CurrentDeck.GetDeck().Count + "cards";
        _index2DeckCount.text = _player2.CurrentDeck.GetDeck().Count + "cards";
        _roundCount.text = "R: " + _gameController.GetRoundNumber();
    }

    IEnumerator DrawCards(Player _whichPlayer)
    {
        for (int i = 0; i < _whichPlayer.CurrentHand.GetCardsInHand.Count; i++)
        {
            //delay between each card spawn for effect
            yield return new WaitForSeconds(.2f);
            GameObject go = new GameObject("Card Container", typeof(RectTransform));
            GameObject card = Instantiate(_cardPrefab, _deck1Transform.position, Quaternion.identity);
            card.transform.SetParent(_board);
            if (_whichPlayer == _player1) go.transform.SetParent(_index1CardContainer);
            else go.transform.SetParent(_index2CardContainer);
            card.GetComponent<CardController>().PouplateCard(_whichPlayer.CurrentHand.GetCardsInHand[i]);
            card.GetComponent<CardController>().SetTargetTransform(go.transform);
            if (_whichPlayer == _player1) _player1Cards.Add(card.GetComponent<CardController>());
            else _player2Cards.Add(card.GetComponent<CardController>());
        }
    }


    // disables cards based on attacking or defending
    public void FlipCards(int attackIndex)
    {
        if (attackIndex == 1) // p1
        {
            for (int i = 0; i < _player1Cards.Count; i++)
            {
                if (_player1Cards[i].HostedCard.IsAttackingCard() == false)
                {
                    _player1Cards[i].GetComponent<Image>().color = Color.gray;
                }
            }

            for (int i = 0; i < _player2Cards.Count; i++)
            {
                if (_index2CardContainer.GetChild(i).GetComponent<CardController>().HostedCard.IsAttackingCard() == true)
                {
                    _index2CardContainer.GetChild(i).GetComponent<Image>().color = Color.gray;
                }
            }
        }
        else if (attackIndex == 2) // p2
        {
            for (int i = 0; i < _player2Cards.Count; i++)
            {
                if (_index2CardContainer.GetChild(i).GetComponent<CardController>().HostedCard.IsAttackingCard() == false)
                {
                    _index2CardContainer.GetChild(i).GetComponent<Image>().color = Color.gray;
                }
            }

            for (int i = 0; i < _player1Cards.Count; i++)
            {
                if (_player1Cards[i].HostedCard.IsAttackingCard() == true)
                {
                    _player1Cards[i].GetComponent<Image>().color = Color.gray;
                }
            }
        }
        else if (attackIndex == 3) // reset cards
        {
            for (int i = 0; i < _player2Cards.Count; i++)
            {
                _index2CardContainer.GetChild(i).GetComponent<Image>().color = Color.black;
            }

            for (int i = 0; i < _player1Cards.Count; i++)
            {
                _player1Cards[i].GetComponent<Image>().color = Color.black;
            }
        }
    }


    public void UpdateUI()
    {
        _index1HP.text = _player1.CurrentHP + "hp";
        _index2HP.text = _player2.CurrentHP + "hp";

        _index1DeckCount.text = _player1.CurrentDeck.GetDeck().Count + "cards";
        _index2DeckCount.text = _player2.CurrentDeck.GetDeck().Count + "cards";
        _roundCount.text = "R: " + _gameController.GetRoundNumber();
    }

    public void DisplayWinner(Player player)
    {
        Debug.Log("WINNER: " + player.PlayerPepemon.DisplayName);
    }
}
