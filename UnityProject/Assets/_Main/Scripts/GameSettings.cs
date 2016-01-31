using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class GameSettings : ScriptableObject {

	public RectTransform 	ZoneUIPrefab;
	public RectTransform 	LetterPrefab;
	public Player 			PlayerPrefab;
	public ZoneEffect[] 	Effects;
	public Color [] 		ZonesColors;
	public Color [] 		PlayerColors;
	public float 			DurationGame;
	public Zone []			ZonePrefabs;
}
