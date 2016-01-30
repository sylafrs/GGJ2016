using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Zone : MonoBehaviour {

	public ZoneEffect Effect;
	public Player Owner { get; private set; }
	private List<Player> Visitors;

	private RectTransform ZoneUI;
	private Queue<KeyCode> Combinaison;

	public bool CanBeTakenOver { get { return Combinaison.Count == 0; } }

	public Zone() : base()
	{
		// Can't call Unity functions here.
		Combinaison = new Queue<KeyCode> ();
		Visitors 	= new List<Player> ();
	}

	public void PlaceUI(RectTransform prefab)
	{
		Assert.Check (prefab, "Zone UI prefab is null");

		GameObject gCanvas = GameObject.Find ("CanvasZone");
		Assert.Check (gCanvas, "CanvasZone gameObject is not found");

		GameObject zoneG = GameObject.Instantiate (prefab.gameObject) as GameObject;
		Assert.Check (zoneG, "Couldn't instantiate zone UI");

		ZoneUI = zoneG.transform as RectTransform;
		Assert.Check (ZoneUI, "Zone has no RectTransform");

		Canvas canvas = gCanvas.GetComponent<Canvas> ();
		Assert.Check (canvas, "CanvasZone's 'Canvas' component is not found");

		ZoneUI.parent = canvas.transform;

		Renderer renderer = this.GetComponent<Renderer> ();
		Assert.Check (renderer, this.name + "'s 'Renderer' component is not found");

		Bounds b = renderer.bounds;
		Vector2 screenMin = RectTransformUtility.WorldToScreenPoint (Camera.main, b.min);
		Vector2 screenMax = RectTransformUtility.WorldToScreenPoint (Camera.main, b.max);

		Vector2 localMin;
		Vector2 localMax;
		RectTransformUtility.ScreenPointToLocalPointInRectangle (canvas.transform as RectTransform, screenMin, Camera.main, out localMin);
		RectTransformUtility.ScreenPointToLocalPointInRectangle (canvas.transform as RectTransform, screenMax, Camera.main, out localMax);

		Rect r = new Rect();
		r.min = localMin;
		r.max = localMax;

		ZoneUI.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, r.width);
		ZoneUI.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, r.height);
		ZoneUI.anchoredPosition = r.center;
	}

	public void AddKeyToCombinaison(KeyCode c)
	{
		Combinaison.Enqueue (c);
	}

	public void OnTriggerEnter(Collider c)
	{
		Player p;
		if (p = c.GetComponent<Player> ()) {
			Visitors.Add(p);
			p.OnEnterZone (this);
		}
	}

	public void OnTriggerExit(Collider c)
	{
		Player p;
		if(p = c.GetComponent<Player>()){
			Visitors.Remove(p);
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
	}

	public void GameUpdate ()
	{
		if (!CanBeTakenOver) {
			KeyCode next = Combinaison.Peek ();
			if (Input.GetKeyDown (next) && Visitors.Count != 0) {
				Combinaison.Dequeue ();
			}
		}
	}

	public void SetColor(Color c)
	{
		this.GetComponent<Renderer> ().material.color = c;
	}
}
