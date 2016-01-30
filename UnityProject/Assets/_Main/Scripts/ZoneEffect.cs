using UnityEngine;
using System.Collections;

public abstract class ZoneEffect : MonoBehaviour 
{
	public abstract void ApplyEffect(Player player);
	public abstract void ApplyInputBuff(ref PlayerInput data);
}
