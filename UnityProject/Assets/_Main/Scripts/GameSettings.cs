using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class GameSettings : ScriptableObject {

	public RectTransform ZoneUIPrefab;
	public RectTransform LetterPrefab;
	public Player PlayerPrefab;
	public ZoneEffect[] Effects;
}
