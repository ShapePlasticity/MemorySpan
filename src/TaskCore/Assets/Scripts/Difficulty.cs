using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Difficulty
{
    private int _lowestLevel = 1;
    private int _currentLevel = 1;

    private int _numberCorrectAtThisLevel = 0;
    private int _numberIncorrectAtThisLevel = 0;
    private int _totalCorrect = 0;
    private int _totalIncorrect = 0;

    //a public property that takes as input the startinglevel from taskengine
    public Difficulty(int startingLevel)
    {
        _currentLevel = startingLevel;
    }

    //returns an int (the _currentLevel) and takes input from AttemptEval as to whether the previous answer was correct or not
    public int ProcessAttempt(AttemptEval result)
    {
        if (result == AttemptEval.Incorrect)
        {
            _numberIncorrectAtThisLevel++;
            _totalIncorrect++;
        }
        else
        {
            _numberCorrectAtThisLevel++;
            _totalCorrect++;
        }

        if (_numberIncorrectAtThisLevel + _numberCorrectAtThisLevel >= 5)
        {
            float performance = ((float)_numberCorrectAtThisLevel / (float)(_numberCorrectAtThisLevel + _numberIncorrectAtThisLevel));
            if (performance < 0.7)
            {
                if (_currentLevel != _lowestLevel)
                    _currentLevel--;
                
                _numberCorrectAtThisLevel = 0;
                _numberIncorrectAtThisLevel = 0;
                
            }
            else
            {
                if (performance > 0.9)
                {
                    _currentLevel++;
                    _numberCorrectAtThisLevel = 0;
                    _numberIncorrectAtThisLevel = 0;
                }
            }
        }

        return _currentLevel;
    }
}

public enum AttemptEval
{
    Init,
    Correct,
    Incorrect
}

