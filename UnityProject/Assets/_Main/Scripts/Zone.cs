using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Zone : MonoBehaviour {

	public ZoneEffect Effect;
	public Player Owner { get; private set; }
	private List<Player> Visitors;

	private RectTransform ZoneUI;
	private List<KeyCode> Combinaison;
	private List<RectTransform> Letters;
	private int CombinaisonStatus;

	// public Renderer renderer;

	public bool CanBeTakenOver { get { return CombinaisonStatus >= Combinaison.Count; } }

	public Zone() : base()
	{
		// Can't call Unity functions here.
		Combinaison = new List<KeyCode> ();
		Visitors 	= new List<Player> ();
		Letters 	= new List<RectTransform> ();
	}

	public void PlaceUI(RectTransform prefabZone, RectTransform prefabLetter)
	{
		Assert.Check (prefabZone, "Zone UI prefab is null");
		Assert.Check (prefabLetter, "Letter prefab is null");

		GameObject gCanvas = GameObject.Find ("CanvasZone");
		Assert.Check (gCanvas, "CanvasZone gameObject is not found");

		GameObject zoneG = GameObject.Instantiate (prefabZone.gameObject) as GameObject;
		Assert.Check (zoneG, "Couldn't instantiate zone UI");

		ZoneUI = zoneG.transform as RectTransform;
		Assert.Check (ZoneUI, "Zone has no RectTransform");

		Canvas canvas = gCanvas.GetComponent<Canvas> ();
		Assert.Check (canvas, "CanvasZone's 'Canvas' component is not found");

		ZoneUI.SetParent(canvas.transform, false);

		Vector2 local;
		Vector2 screen = RectTransformUtility.WorldToScreenPoint (Camera.main, this.transform.position);
		RectTransformUtility.ScreenPointToLocalPointInRectangle (canvas.transform as RectTransform, screen, Camera.main, out local);
		ZoneUI.anchoredPosition = local;

		for (int i = 0; i < Combinaison.Count; i++) 
		{
			GameObject letterG = GameObject.Instantiate (prefabLetter.gameObject) as GameObject;
			Assert.Check (zoneG, "Couldn't instantiate letter");

			RectTransform letter = letterG.transform as RectTransform;
			Assert.Check (ZoneUI, "Letter has no RectTransform");

			letter.SetParent (ZoneUI, false);
			Letters.Add (letter);

			RectTransform txtT = letter.FindChild ("Text") as RectTransform;
			Assert.Check (txtT, "Letter Text child not found or has no RectTransform");

			Text txt = txtT.GetComponent<Text> ();
			Assert.Check (txtT, "Letter Text child's 'Text' component not found");

			txt.text = Combinaison [i].ToString();
		}
	}

	public void AddKeyToCombinaison(KeyCode c)
	{
		Combinaison.Add (c);
		CombinaisonStatus = 0;
		foreach (RectTransform rt in Letters)
			rt.gameObject.SetActive (true);
	}

	public void OnTriggerEnter(Collider c)
	{
		Player p;
		if (p = c.GetComponent<Player> ()) 
		{
			if (p.Position != null)
				p.Position.Visitors.Remove (p);
			Visitors.Add(p);
			p.OnEnterZone (this);
		}
	}

	public void OnTriggerExit(Collider c)
	{
		Player p;
		if(p = c.GetComponent<Player>()){
			//Visitors.Remove(p);
			p.OnLeaveZone (this);
		}
	}

	public void OnPlayerTakeOver(Player p)
	{
		Assert.Check (Visitors.Contains (p), "Visitors doesn't contain the player that take over");
		Assert.Check (CanBeTakenOver, "Can't be taken over");

		this.Owner = p;
		Game.Instance.StartCoroutine(this.Effect.ApplyEffect (p));

		this.SetColor (p.color);
		this.CombinaisonStatus = 0;
	}

	public void GameUpdate ()
	{
		if (!CanBeTakenOver)
		{
			KeyCode next = Combinaison[CombinaisonStatus];
			if (Input.GetKeyDown (next) && Visitors.Count != 0) {
				Letters [CombinaisonStatus].gameObject.SetActive (false);
				CombinaisonStatus++;
			}
		}
	}

	public void SetColor(Color c)
	{
		if(this.GetComponent<Renderer> ())
			this.GetComponent<Renderer> ().material.color = c;
	}
}
