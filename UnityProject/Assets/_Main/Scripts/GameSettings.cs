using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class GameSettings : ScriptableObject {

	public RectTransform 	ZoneUIPrefab;
	public RectTransform 	LetterPrefab;
	public Player[]			PlayerPrefabs;
	public ZoneEffect[] 	Effects;
	public Color [] 		ZonesColors;
	public Color [] 		PlayerColors;
	public float 			DurationGame;
	public Zone []			ZonePrefabs;
}
