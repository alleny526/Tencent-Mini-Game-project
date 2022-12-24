using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    [System.Serializable]
    public enum Turn
    {
        Player,
        Enemy,
        Null,
    }

    public Turn _gameTurn = Turn.Null;

    public int _turnIndex = 0;

    public int _playerMoney = 10;
    [HideInInspector]
    public int _playerMoneyCurrent = 10;
    [HideInInspector]
    public int _costDrawn = 0;

    public CardManager _cardManager;
    public ComboManager _comboManager;
    public Player _player;
    public Boss_Parent _boss;

    public Canvas MainCanvas;

    public GameObject _playBtn;
    public GameObject _moneyPic;
    public GameObject _playerHPBar;
    public GameObject _bossHPBar;
    public GameObject _textPrefab;


    // Start is called before the first frame update
    void Start()
    {
        _turnIndex = 0;
        _cardManager = GameObject.Find("CardManager").GetComponent<CardManager>();
        _comboManager = GameObject.Find("ComboManager").GetComponent<ComboManager>();
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _boss = GameObject.FindGameObjectWithTag("Boss").GetComponent<Boss_Parent>();
        MainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();
        _playBtn = GameObject.Find("PlayBtn");
        _moneyPic = GameObject.Find("Money");

        // _player.SetActive(true);
        // _boss.SetActive(true);
        _moneyPic.SetActive(true);
        _playBtn.SetActive(true);
        _playBtn.GetComponent<Button>().enabled = false;


        NewTurn(_player);
        
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] _cardOnCase = GameObject.FindGameObjectsWithTag("Card");
        if(_cardOnCase.Length > 0)
        {
            bool _drawnAtLeastOnce = false;
            int costDrawn = 0;
            foreach (GameObject card in _cardOnCase)
            {
                Card c = card.GetComponent<Card>();
                if(c._drawn == true)
                {
                    _drawnAtLeastOnce = true;
                    costDrawn += c._cardCost;
                }
            }
            _playBtn.GetComponent<Button>().enabled = ((costDrawn <= _playerMoney) && _drawnAtLeastOnce);
            if (costDrawn <= _playerMoney) {
                this._costDrawn = costDrawn;
            }

        }

        UpdateHPBar();
        
    }

    public void UpdateMoneyCurrent(int newMoney) {
        _playerMoneyCurrent = newMoney;

        Sprite newImage = Resources.Load("UI/Windows/MainWindow/UIsources/price/"+newMoney, typeof(Sprite)) as Sprite;
        Debug.Log("new image: "+newImage);
        _moneyPic.GetComponent<Image>().sprite = newImage;
        Debug.Log("New Money = " + newMoney + ", Money remaining: $"+this._playerMoneyCurrent);
    }

    public void OnPlayBtnClicked() {
        Debug.Log("PlayBtn Clicked");
        // 1. cost calculation
        UpdateMoneyCurrent((this._playerMoneyCurrent - _costDrawn));

        // 2. play
        PlayCard(_player);

        // 3. reset and redeal cards
        StartCoroutine(WaitAndNewTurn());
    }

    private IEnumerator WaitAndNewTurn() {
        yield return new WaitForSeconds(5);
        NewTurn(_player);
    }

    public void NewTurn(Role who)
    {
        _playBtn.GetComponent<Button>().enabled = false;

        _player._cardList.Clear();
        _boss._cardList.Clear();
        UpdateMoneyCurrent(_playerMoney);

        foreach (var item in GameObject.FindGameObjectsWithTag("Card"))
        {
            GameObject.Destroy(item);
        }

        _turnIndex += 1;
        if(who.gameObject.tag == "Player")
        {
            _gameTurn = Turn.Player;
        }
        else if(who.gameObject.tag == "Boss")
        {
            _gameTurn = Turn.Enemy;
        }
        DealCard(who);
    }

    public void DealCard(Role who)
    {
        who._cardList.Clear();

        if(_cardManager._cardList.Count > 0)
        {
            for (int i = 0; i < 5; i++)
            {
                int index = Random.Range(0, _cardManager._cardList.Count);
                who._cardList.Add(_cardManager._cardList[index]);
                // if(who.tag == "Player")
                // {
                    GameObject _card = GameObject.Instantiate(_cardManager._cardList[index], MainCanvas.transform);
                    _card.GetComponent<RectTransform>().position = new Vector3(
                        MainCanvas.transform.position.x-5.4f+i*2.3f,
                        MainCanvas.transform.position.y-3.2f,
                        MainCanvas.transform.position.z
                        );
                    // Debug.Log(_cardManager._cardList[index].GetComponent<Card>()._cardName + _card.GetComponent<RectTransform>().position);
                // }

            }
            Debug.Log("card dealed");
        }


    }

    public void PlayCard(Role who) {
        // 1. 玩家攻防
        GameObject[] _cardOnCase = GameObject.FindGameObjectsWithTag("Card");
        List<Card> _cardOnDraw = new List<Card>();
        List<string> _cardsDrawn = new List<string>{};

        if (_cardOnCase.Length > 0) {
            // save drawn cards to _cardOnDraw
            foreach (GameObject card in _cardOnCase) {
                if (card.GetComponent<Card>()._drawn == true) {
                    _cardOnDraw.Add(new Card(card.GetComponent<Card>()));
                    _cardsDrawn.Add(card.GetComponent<Card>()._cardClassName);
                }
            }
            
            // find combos and do actions
            foreach (Combo combo in _comboManager.comboList) {
                List<int> desc_indexes = matchCombo(combo._cardClassNames, _cardsDrawn);
                if (desc_indexes.Count > 0) {
                    Debug.Log("Combo found");
                    foreach (int index in desc_indexes) {
                        _cardOnDraw[index].OnUse(who, true);    // individual action of combo cards
                        who._pendingAnimations.Add(_cardOnDraw[index]._cardClassName.Replace("Card_", ""));
                        _cardOnDraw.RemoveAt(index);
                        _cardsDrawn.RemoveAt(index);
                    }
                    combo.OnUse(who);   // bonus action or buff of combo cards
                }
            }
            // do individual actions of non-combo cards
            foreach (Card card in _cardOnDraw) {
                card.OnUse(who, false);
                who._pendingAnimations.Add(card._cardClassName.Replace("Card_", ""));
            }

        }

        // 2. BOSS攻防
        _boss.NextBehavior();

        // TODO: animations
        who.PlayAnimations();

        // 3. 计算攻防结果
        who._opponent.Calculate();
        // TODO: UI更新双方血条盾条

        // 4. 加载卡牌给下一回合的buff
        if (_cardOnCase.Length > 0) {
            foreach (GameObject card in _cardOnCase) {
                if (card.GetComponent<Card>()._drawn == true) {
                    card.GetComponent<Card>().OnNextRound(who);
                }
            }
        }
    }

    private List<int> matchCombo(List<string> combo, List<string> drawn) {
        List<int> index_list = new List<int>();
        foreach (string name in combo) {
            int ret = drawn.IndexOf(name);
            if (ret == -1) {
                index_list.Clear();
                break;
            }
            else {
                index_list.Add(ret);
            }
        }
        
        index_list.Sort();
        index_list.Reverse();
        return index_list;
    }


    public void UpdateHPBar()
    {
        this._playerHPBar.GetComponent<Image>().fillAmount = _player._hitPointCurrent/_player._hitPoint;
        this._bossHPBar.GetComponent<Image>().fillAmount = _boss._hitPointCurrent/_boss._hitPoint;
    }

    public void BurstText(GameObject obj, float damage)
    {
        GameObject _newUI = GameObject.Instantiate(_textPrefab, MainCanvas.gameObject.transform);
        _newUI.GetComponent<RectTransform>().localPosition = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().WorldToScreenPoint(obj.gameObject.transform.position);
        _newUI.GetComponent<newUI>().UINumber = damage;

    }

}
