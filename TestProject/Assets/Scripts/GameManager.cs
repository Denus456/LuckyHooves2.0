using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{ 
    [SerializeField] private Animator _mainMenuSubPanel;
    [SerializeField] private Animator _gamePlayPanel;

    [SerializeField] private HorseGenerator _horseGenerator;
    [SerializeField] private ObjectHorizontalGenerator _tribuneGenerator;

    [SerializeField] private Button _defaulLineButton;
    [SerializeField] private Button _defaulBetButton;

    [SerializeField] private GameObject _menuPanel;

    [SerializeField] private GameObject _horseNumberButtonPrefab;
    [SerializeField] private GameObject _horseNumberButtonsPlace;

    [SerializeField] private GameObject _startRaceButton;

    [SerializeField] private GameObject _raceInfoPanel;
    [SerializeField] private TextMeshProUGUI _summary;
    [SerializeField] private TextMeshProUGUI _winSummary;
    [SerializeField] private TextMeshProUGUI _countdownText;

    [SerializeField] private List<ParticleSystem> _winConfettis;

    [SerializeField] private CameraFollow _cameraFollow;

    [SerializeField] private GameObject _winInfoPanel;

    [SerializeField] private AudioMixer _audioMixer;

    private List<HorseController> _horses;
    private List<GameObject> _horseNumberButtons = new List<GameObject>();

    private float countdownTime = 3f;

    private Button _currentLineButton;
    private Button _currentBetButton;
    private Button _currentHorseNumberButton;

    private HorseLineButton[] _horseLineButtons;
    private BetAmountButton[] _betAmountButtons;

    private int _horseLinesCount = 3;
    private int _betAmount = 100;

    private int _horseFinishedCounter = 0;

    private int _selectedHorseFinishedNumber = 0;

    private int _selectedHorseNumber;

    public void ChangeMusicVolume(float level)
    {
        _audioMixer.SetFloat("musicValue", level > 0 ? MathF.Log10(level) * 20 : -80f);
    }
    public void ChangeSoundEffectsVolume(float level)
    {
        _audioMixer.SetFloat("soundEffectsValue", level > 0 ? MathF.Log10(level) * 20 : -80f);
    }


    public void GameRestart()
    {
        _horseFinishedCounter = 0;
        _selectedHorseFinishedNumber = 0;

        _horseGenerator.Restart();

        _gamePlayPanel.gameObject.SetActive(true);
        _menuPanel.SetActive(true);

        _horseNumberButtons.ForEach(horse => { Destroy(horse.gameObject); });
        _horseNumberButtons.Clear();

        _currentHorseNumberButton = null;
        _startRaceButton.SetActive(false);
    }

    public void GenerateHorseNumberButtons()
    {
        for (int i = 1; i <= _horseLinesCount; i++)
        {
            int horseNumber = i;
            GameObject button = Instantiate(_horseNumberButtonPrefab, _horseNumberButtonsPlace.transform);
            button.GetComponent<Button>().onClick.AddListener(() => SetSelectedHorseHumber(horseNumber));
            button.GetComponent<Button>().onClick.AddListener(() => SetCurrentHorseNumberButton(button.GetComponent<Button>()));
            button.GetComponentInChildren<TextMeshProUGUI>().text = horseNumber.ToString();
            _horseNumberButtons.Add(button);
        }
    }

    public void GoBack()
    {
        _mainMenuSubPanel.gameObject.SetActive(true);
        _mainMenuSubPanel.SetTrigger("MenuChange");
    }

    public void PlayGame(GameObject playPanel)
    {
        _mainMenuSubPanel.SetTrigger("MenuDisappear");
        playPanel.SetActive(true);
    }

    public void GameQuit()
    {
        Application.Quit();
    }

    public void SetHorseLines(int count) => _horseLinesCount = count;
    public void SetBetAmount(int count) => _betAmount = count;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        _mainMenuSubPanel.SetTrigger("GameStart");

        _horseLineButtons = FindObjectsOfType<HorseLineButton>(true);
        _betAmountButtons = FindObjectsOfType<BetAmountButton>(true);

        foreach (var button in _horseLineButtons)
        {
            button.GetComponent<Button>().onClick.AddListener(() => SetCurrentLineButton(button.GetComponent<Button>()));
        }

        foreach (var button in _betAmountButtons)
        {
            button.GetComponent<Button>().onClick.AddListener(() => SetCurrentBetButton(button.GetComponent<Button>()));
        }

        _tribuneGenerator.GenerateObjects();

        _currentLineButton = _defaulLineButton;
        _currentBetButton = _defaulBetButton;
    }

    private void SetCurrentLineButton(Button button)
    {
        if (_currentLineButton != button)
        {
            _currentLineButton.GetComponent<Animator>().SetTrigger("Deselected");
            _currentLineButton = button;
            _currentLineButton.GetComponent<Animator>().SetTrigger("Selected");
        }
    }

    private void SetCurrentBetButton(Button button)
    {
        if (_currentBetButton != button)
        {
            _currentBetButton.GetComponent<Animator>().SetTrigger("Deselected");
            _currentBetButton = button;
            _currentBetButton.GetComponent<Animator>().SetTrigger("Selected");
        }
    }

    private void SetCurrentHorseNumberButton(Button button)
    {
        if (_currentHorseNumberButton != button)
        {
            _currentHorseNumberButton?.GetComponent<Animator>().SetTrigger("Deselected");
            _currentHorseNumberButton = button;
            _currentHorseNumberButton.GetComponent<Animator>().SetTrigger("Selected");
            _startRaceButton.SetActive(true);
        }
    }

    private void SetSelectedHorseHumber(int number) => _selectedHorseNumber = number;
    public void GenerateLevel()
    {
        _horses = _horseGenerator.GenerateHorsesWithFences(_horseLinesCount, _selectedHorseNumber);

        StartCoroutine(CountdownRoutine());
    }
    private IEnumerator CountdownRoutine()
    {
        _countdownText.gameObject.SetActive(true);

        for (int i = (int)countdownTime; i > 0; i--)
        {
            _countdownText.text = i.ToString();
            AnimateText(); // Apply DOTween animation
            yield return new WaitForSeconds(1f);
        }

        _countdownText.text = "GO!!!";
        AnimateText();
        yield return new WaitForSeconds(1f);

        _horses.ForEach(horse => { horse.StartRuning(); });

        _countdownText.DOFade(0f, 0.5f).OnComplete(() => _countdownText.gameObject.SetActive(false)); // Fade out
    }

    private void AnimateText()
    {
        _countdownText.transform.localScale = Vector3.one * 2f; // Start big
        _countdownText.DOFade(1f, 0.2f); // Fade in
        _countdownText.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce); // Smooth bounce effect
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Horse"))
        {
            _horseFinishedCounter++;
            if (_selectedHorseNumber == other.GetComponent<HorseController>().HorseNumber)
            {
                _selectedHorseFinishedNumber = _horseFinishedCounter;
                _cameraFollow.SetTarget(null);
                HorseFinished();
            }
        }
    }

    private void HorseFinished()
    {
        if (_selectedHorseFinishedNumber != 1)
        {
            _summary.text = $"Your horse took {_selectedHorseFinishedNumber} place";
            _raceInfoPanel.SetActive(true);
        }
        else
        {
            _winConfettis.ForEach(winConfetti => { winConfetti.Play(); });
            int reward = _horseLinesCount * _betAmount;
            _winSummary.text = $"You win {reward}";
            _winInfoPanel.SetActive(true);
        }
    }
}
