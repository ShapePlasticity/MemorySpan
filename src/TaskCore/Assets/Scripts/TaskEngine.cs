using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class TaskEngine : MonoBehaviour
{
    private TaskSettings _gameManager;
    private Sprite _feedbackSprite;
    private Difficulty _difficulty;
    private ScannerHandler _scannerIn;
    private KeyboardHandler _keyInput;

    private IPerfLog _logger = new FileSystemLogger();
    private AttemptEval _currentRespEval = AttemptEval.Init;

    public TaskType CurrentTaskType = TaskType.Standard;
    public TaskState CurrentTaskState = TaskState.RunTask;
    public BlockType CurrentBlockType = BlockType.TrialBlock;

    #region Public Visual Sprites & Objects
    public GameObject MainGO;
    public GameObject ChoiceTemplate;
    public GameObject Choices;

    public Sprite Check;
    public Sprite Cross;
    public Sprite Hourglass;
    #endregion

    private DateTime _scanTime = DateTime.Now;
    private DateTime _digitDisplayTimer = DateTime.Now;
    private long _responseTimer = 0;
    private DateTime _reinforcementTimer = DateTime.Now;

    private int _taskTimeLimit = 90; //total time the trial will run for (sec)
    private int _digitTimeout = 300;//how long each digit is displayed on screen (msec)
    public float FeedbackTimeout = 300;
    private int _responseTimeLimit = 4; //amount of time alloted for user to make response

    private int _randomIndex; //random ints used to index into sprite array
    private int _startingSpanLength = 2; //the # of digits that will be presented on the initial iteration
    private int _currentSpanLength = 0; // tracks current length of the digit span
    private int _maxSpanLimit = 4; //maximum span length allowed on trial
    private string _spriteFileName = "Sprites/Numbers";
    private int _digitsCorrect = 0; //number of digits correctly placed in an overall incorrect response

    public long TrialStartTime = 0;
    public long TrialEndTime = 0;
    public long TrialDuration = 0;
    public long BlockStartTime = 0;
    //private long _presentStartTime = 0;
    //private long _presentEndTime = 0;
    //public long PresentDuration = 0;
    public long TrialTimeLimit = 10000;

    public int MaxSpan = 0; //maximun DigitSpan reached by user
    private int _iterationCount = 0;

    public Sprite[] _spriteArray;//array that holds sprites
    List<int> _spanLengths = new List<int>(); //holds all span lengths attempted by user
    List<int> _randomIndexArray = new List<int>(); //an array to hold a string of randomly generated ints, used to index into _spriteArray
    List<int> _responseArray = new List<int>(); //holds the user repsonses 

    void Awake()
    {
        _gameManager = TaskSettingsManager.TaskSettings;

        _scannerIn = FindObjectOfType<ScannerHandler>();
        _keyInput = FindObjectOfType<KeyboardHandler>();

        if (_gameManager.EnableTriggerOnScanner == true)
        {
            CurrentTaskState = TaskState.WaitForTrigger;
            CurrentTaskType = TaskType.Scanner;
            CurrentBlockType = BlockType.BaselineBlock;
        }

        _spriteFileName = _gameManager.SpriteFileName;
        int.TryParse(_gameManager.StartingSpan, out _startingSpanLength);
        int.TryParse(_gameManager.TaskDuration, out _taskTimeLimit);
        int.TryParse(_gameManager.MaxSpan, out _maxSpanLimit);
        int.TryParse(_gameManager.ResponseTimeLimit, out _responseTimeLimit);

        Random.seed = (int)(Time.time * 10000);
        _currentSpanLength = _startingSpanLength; //makes the current span length equal to the starting span length

        //IMPORTANT: loads thatever is in the resource's folder to the array
        _spriteArray = Resources.LoadAll<Sprite>("Sprites/Numbers");

        var spritePadding = 15f * (_spriteArray.Length-1);
        var spriteWidth = 0f;

        //setting up the reponse template that user uses?
        for (int i = 0; i < _spriteArray.Length; i++)
        {
            //an object created in memory that is attached to TaskEngineGO
            var choiceItem = Instantiate(ChoiceTemplate);

            // Set the choice handler index and response handler
            var ciHandler = choiceItem.GetComponent<ChoiceHandler>();
            ciHandler.ItemIndex = i;
            ciHandler.ResponseHandler = _responseArray;

            // Set the sprite
            var ciSprite = choiceItem.GetComponent<SpriteRenderer>();
            ciSprite.sprite = _spriteArray[i];

            // Change the collider to fit the sprite
            var boxCollider = choiceItem.GetComponent<BoxCollider2D>();
            boxCollider.size = new Vector2(ciSprite.bounds.size.x, ciSprite.bounds.size.y);
            boxCollider.offset += new Vector2(ciSprite.bounds.size.x / 2, ciSprite.bounds.size.y / 2);

            // Move the item to the correct spot in the parent
            float spriteLeft = spriteWidth + (i * 15);
            choiceItem.transform.SetParent(Choices.transform, false);
            choiceItem.transform.localPosition = new Vector3(spriteLeft, 0, 0);

            // Track how much total width our sprites have
            spriteWidth += _spriteArray[i].bounds.size.x;
        }

        //communicating the starting span length to our new instance of Difficulty
        _difficulty = new Difficulty(_startingSpanLength); 

        // Change the camera to be the right size based on sprite sheet width
        var cam = Camera.main;
        cam.orthographicSize = (spriteWidth + spritePadding) * Screen.height / Screen.width * 0.5f;

        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;

        var cameraLeft = cam.transform.localPosition.x - (width / 2);
        var cameraBottom = cam.transform.localPosition.y - (height / 2); ;

        Choices.transform.position = new Vector3(cameraLeft, cameraBottom);

    }

    void Start()
    {   
        StartCoroutine(RunTask());
    }

    //checks if Task is running within Time/Span Limits
    //TODO: will need external clock if we need task to end at predictable point (scanner)
    IEnumerator RunTask()
    {
        if (CurrentTaskType == TaskType.Scanner)
        {
            while (CurrentTaskState == TaskState.WaitForTrigger)
            {
                Choices.SetActive(false);

                _scannerIn.WaitForTrigger();
                yield return new WaitForSeconds(.005f);
            }

            _logger.Init();

            while ((_currentSpanLength < _maxSpanLimit) && (Time.timeSinceLevelLoad < _taskTimeLimit))
            {
                yield return StartCoroutine(ShowSpan());
                yield return StartCoroutine(CollectResponses());
            }
        }

        if (CurrentTaskType == TaskType.Standard)
        {
            _logger.Init();

            while ((_currentSpanLength < _maxSpanLimit) && (Time.timeSinceLevelLoad < _taskTimeLimit))
            {
                yield return StartCoroutine(ShowSpan());
                yield return StartCoroutine(CollectResponses());
            }
        }

        _logger.Term();
        SceneManager.LoadScene("GameEndScreen");
    }

    //display the span to user
    IEnumerator ShowSpan()
    {
        //TODO:BLOCK TIMING
        BlockStartTime = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);

        if (CurrentBlockType == BlockType.TrialBlock)
        {
            TrialStartTime = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);

            Choices.SetActive(false); //setting the response buttons inactive while Span displayed
            MainGO.SetActive(false);

            _randomIndexArray.Clear();
            _responseArray.Clear();

            for (int i = 0; i < _currentSpanLength; i++)
            {
                _randomIndex = Random.Range(0, _spriteArray.Length);
                _randomIndexArray.Add(_randomIndex); //an array of randomly selected ints used to tag the digit sprites themselves 
            }

            for (int i = 0; i < _currentSpanLength; i++)
            {
                // Delay this next digit if it is not the first (200 ms wait in between digit presentation if not first digit)
                if (i != 0)
                    //ISI
                    yield return new WaitForSeconds(200 / 1000f);

                // Load the sprite and show it for digit timeout in msec
                MainGO.GetComponent<SpriteRenderer>().sprite = _spriteArray[_randomIndexArray[i]];
                MainGO.SetActive(true);

                yield return new WaitForSeconds(_digitTimeout / 1000f);

                MainGO.SetActive(false);
            }
        }

        if (CurrentBlockType == BlockType.BaselineBlock)
        {
            TrialStartTime = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);

            Choices.SetActive(false); //setting the response buttons inactive while Span displayed
            MainGO.SetActive(false);

            _randomIndexArray.Clear();
            _responseArray.Clear();

            for (int i = 0; i < _currentSpanLength; i++)
            {
                _randomIndex = Random.Range(0, _spriteArray.Length);
                _randomIndexArray.Add(_randomIndex); //an array of randomly selected ints used to tag the digit sprites themselves 
            }

            for (int i = 0; i < _currentSpanLength; i++)
            {
                // Delay this next digit if it is not the first (200 ms wait in between digit presentation if not first digit)
                if (i != 0)
                    //ISI
                    yield return new WaitForSeconds(200 / 1000f);

                // Load the sprite and show it for digit timeout in msec
                MainGO.GetComponent<SpriteRenderer>().sprite = _spriteArray[_randomIndexArray[i]];
                MainGO.SetActive(true);

                yield return new WaitForSeconds(_digitTimeout / 1000f);

                MainGO.SetActive(false);
            }
        }

    }

    IEnumerator CollectResponses()
    {
        // Enable the buttons
        Choices.SetActive(true);

        _responseTimer = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);

        // Check for user responses or timeout
        int previousCount = 0;
        while (true)
        {
            // If we made it this far and our count has increased then we should show the thing they chose
            if (previousCount < _responseArray.Count)
            {
                // Grab the last response
                int itmIdx = _responseArray[_responseArray.Count - 1];

                // Set the glyph
                MainGO.GetComponent<SpriteRenderer>().sprite = _spriteArray[itmIdx];
                MainGO.SetActive(true);
                yield return new WaitForSeconds(0.200f);
                MainGO.SetActive(false);
                yield return new WaitForSeconds(0.100f);

                previousCount++;

                continue;
            }

            // If we have processed all responses or hit our timeout then process and break loop
            if (((_randomIndexArray.Count <= _responseArray.Count)) || (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - _responseTimer)/1000) >= _responseTimeLimit)
            {
                //return of this is eval of answer 
                _iterationCount++;
                _currentRespEval = CheckResponses(); 
                _spanLengths.Add(_currentSpanLength); //comtains all span lengths attempted
                //calculating total trial duration
                TrialEndTime = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
                TrialDuration = (TrialEndTime - TrialStartTime);

                //log response
                ResponseLogger();
                //set span length
                _currentSpanLength = _difficulty.ProcessAttempt(_currentRespEval);

                //if we are in scanner...calculate the appropreate time to display feedback to make trial last x number of sec
                if (CurrentTaskType == TaskType.Scanner)
                {
                    FeedbackTimeout = (TrialTimeLimit - TrialDuration);
                }

                MainGO.GetComponent<SpriteRenderer>().sprite = _feedbackSprite;
                MainGO.SetActive(true);

                yield return new WaitForSeconds(FeedbackTimeout/1000);
                MainGO.SetActive(false);

                break;
            }

            // Pause for 100 msec and check again for response
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.5f);
    }

    //checks if user input matches current Digit span (correct/incorrect)
    //loops through wrong answer and checks how many elements are off --> takes %
    //return an enumaeration state which is used to adjust difficulty
    public AttemptEval CheckResponses()
    {
        // If counts do not match then we timed out
        if (_randomIndexArray.Count == _responseArray.Count)
        {
            if (_randomIndexArray.SequenceEqual(_responseArray) == true) // then check equality of contents
            {
                _feedbackSprite = Check;
                return AttemptEval.Correct;
            }
            else
            {
                for (int i = 0; i < _randomIndexArray.Count; i++) //analyzing the worong response and checking how many digits were wrong, takes a %
                {
                    if (_responseArray[i] == _randomIndexArray[i])
                        _digitsCorrect++;
                }

                _feedbackSprite = Cross;
                return AttemptEval.Incorrect;
            }
        }
        else
        {
            _feedbackSprite = Cross;
            return AttemptEval.Incorrect;
        }
    }

    public void AbortTrial()
    {
        _logger.Term();
    }

    public void ResponseLogger()
    {
        if (CurrentBlockType == BlockType.TrialBlock)
        {
            _logger.LogRaw(_iterationCount + " " + _currentSpanLength + " " + _currentRespEval.ToString() + " " + _spanLengths.Max());
        }
    }
}

public enum TaskType
{
    Standard,
    Scanner
}

public enum TaskState
{
    WaitForTrigger,
    RunTask
}

public enum BlockType
{
    BaselineBlock,
    TrialBlock
}
