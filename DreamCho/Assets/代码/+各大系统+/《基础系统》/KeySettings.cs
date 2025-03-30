using System.Collections.Generic;
using UnityEngine;
using DreamCho;

[CreateAssetMenu(fileName = "Key Binds", menuName = "Key/Key Binds")]
public class KeySettings : ScriptableObject
{
    List<KeyCode> nullKeys = new List<KeyCode>() { KeyCode.None };

    private List<KeyCode> Check(List<KeyCode> defaultKey)
    {
        if (Key.canGetInput)
        {
            return defaultKey;
        }
        else return nullKeys;
    }

    [SerializeField] private List<KeyCode> left = new List<KeyCode> { KeyCode.A };
    public List<KeyCode> Left { get { return Check(left); } set { left = value; } }

    [SerializeField] private List<KeyCode> right = new List<KeyCode> { KeyCode.D };
    public List<KeyCode> Right { get { return Check(right); } set { right = value; } }

    [SerializeField] private List<KeyCode> up = new List<KeyCode> { KeyCode.W };
    public List<KeyCode> Up { get { return Check(up); } set { up = value; } }

    [SerializeField] private List<KeyCode> down = new List<KeyCode> { KeyCode.S };
    public List<KeyCode> Down { get { return Check(down); } set { down = value; } }





    [SerializeField] private List<KeyCode> jump = new List<KeyCode> { KeyCode.Space };
    public List<KeyCode> Jump { get { return Check(jump); } set { jump = value; } }     

    [SerializeField] private List<KeyCode> dash = new List<KeyCode> { KeyCode.K, KeyCode.Mouse1 };
    public List<KeyCode> Dash { get { return Check(dash); } set { dash = value; } }

    [SerializeField] private List<KeyCode> power = new List<KeyCode> { KeyCode.I };
    public List<KeyCode> Power { get { return Check(power); } set { power = value; } }

    [SerializeField] private List<KeyCode> next_character = new List<KeyCode> { KeyCode.Q };
    public List<KeyCode> NextCharacter { get { return Check(next_character); } set { next_character = value; } }

    [SerializeField] private List<KeyCode> prevoius_character = new List<KeyCode> { KeyCode.E };
    public List<KeyCode> PreviousCharacter { get { return Check(prevoius_character); } set { prevoius_character = value; } }

    [SerializeField] private List<KeyCode> interact = new List<KeyCode> { KeyCode.F };
    public List<KeyCode> Interact { get { return Check(interact); } set { interact = value; } }

    [SerializeField] private List<KeyCode> attack = new List<KeyCode> { KeyCode.J };
    public List<KeyCode> Attack { get { return Check(attack); } set { attack = value; } }



    // UI，游戏通用

    [SerializeField] private List<KeyCode> confirm = new List<KeyCode> { KeyCode.Return };
    public List<KeyCode> Confirm { get { return confirm; } set { confirm = value; } }

    [SerializeField] private List<KeyCode> escape = new List<KeyCode> { KeyCode.Backspace };
    public List<KeyCode> Escape { get { return escape; } set { escape = value; } }

    [SerializeField] private List<KeyCode> tab = new List<KeyCode> { KeyCode.Tab };
    public List<KeyCode> Tab { get { return tab; } set { tab = value; } }
}