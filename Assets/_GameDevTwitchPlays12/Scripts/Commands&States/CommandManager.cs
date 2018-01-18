﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameManager;

public class CommandManager : DualBehaviour, ICommandManager
{
    #region Public Var

    public long cd;
    public int maxPlayer;

    public char firstCommmandCharacter  = '!';
    public char firstStateCharacter     = '?';

    public static Command INVALIDCOMMAND = new Command("Votre commande est invalide", true);

    #endregion

    #region Public Func

    public ICommand Parse(string _username, int _plateform, string _message, long _time)
    {
        string userID = _plateform + " " + _username;
        
        if ((!IsACommand(_message)) && (!IsAState(_message)))
        {
            return null;
        }
        _message.ToUpper();
        if ((!CommandIsValid(_message)) && (!StateIsValid(_message)))
        {
            return INVALIDCOMMAND;
        }
        if (_message.Contains("JOIN"))
        {
            if (!userDataBase.ContainsKey(userID))
            {
                if (userDataBase.Count < maxPlayer)
                {
                    userDataBase.Add(userID, 0);
                }
                else
                {
                    return new Command("Le nombre maximum de joueur est atteint", true);
                }
            }
            else
            {
                return new Command("Vous avez déja rejoins la partie", true);
            }
        }
        if ((!Cooldown(_time, userID)) && (!_message.Contains("JOIN")))
        {
            return new Command("Le cooldown entre 2 commandes n'est pas terminé", true);
        }
        if (!userDataBase.ContainsKey(userID))
        {
            return new Command("Veuillez d'abord rejoindre la partie a l'aide de la commande !JOIN", true);
        }
        userDataBase[userID] = _time;
        return new Command(_message, false);
    }

    #endregion

    #region Private Func

    private bool IsACommand(string message)
    {
        bool isValid;
        if (message[0] == firstCommmandCharacter)
        {
            isValid = true;
        }
        else
        {
            isValid = false;
        }
        return isValid;
    }

    private bool IsAState(string message)
    {
        bool isValid;
        if (message[0] == firstStateCharacter)
        {
            isValid = true;
        }
        else
        {
            isValid = false;
        }
        return isValid;
    }

    private bool CommandIsValid(string message)
    {
        bool isValid = false;
        for (int i = 0; i < validCommand.Count; i++)
        {
            if (message.Equals(firstCommmandCharacter + validCommand[i]))
            {
                isValid = true;
                break;
            }
        }
        return isValid;
    }

    private bool StateIsValid(string message)
    {
        bool isValid = false;
        for (int i = 0; i < validCommand.Count; i++)
        {
            if (message.Equals(firstStateCharacter + validState[i]))
            {
                isValid = true;
                break;
            }
        }
        return isValid;
    }

    private bool Cooldown(long time, string name)
    {
        long oldTime;
        if (userDataBase.ContainsKey(name))
        {
            oldTime = userDataBase[name];
        }
        else
        {
            oldTime = 0;   
        }
        long value = time - oldTime; 

        if (value < cd)
        {
            return false;
        }
        return true;
    }

    #endregion

    #region Private Var

    [SerializeField]
    private List<string> validCommand = new List<string>
    {
        "UP"    ,
        "DOWN"  ,
        "LEFT"  ,
        "RIGHT" ,
        "DIG"   ,
        "JOIN"  ,
    };

    [SerializeField]
    private List<string> validState = new List<string>
    {
        "NORMAL"    ,
        "STUN"      ,
        "SPRAIN"    ,
    };

    private Dictionary<string, long> _userDataBase = new Dictionary<string, long>();

    public Dictionary<string, long> userDataBase
    {
        get { return _userDataBase; }
        set { _userDataBase = value; }
    }

    #endregion
}

public class Command:ICommand
{
    private bool _feedbackUser;

    public bool feedbackUser
    {
        get { return _feedbackUser; }
        set { _feedbackUser = value; }
    }

    private string _response;

    public string response
    {
        get { return _response; }
        set { _response = value; }
    }

    public Command(string message, bool feedback)
    {
        feedbackUser = feedback;
        response = message;

    }
}
