using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Zone : MonoBehaviour {

	public ZoneEffect Effect;
	public Player Owner { get; private set; }
	private List<Player> Visitors;

	private Queue<KeyCode> Combinaison;

	public bool CanBeTakenOver { get { return Combinaison.Count == 0; } }

	public Zone() : base()
	{
		// Can't call Unity functions here.
		Combinaison = new Queue<KeyCode> ();
		Visitors 	= new List<Player> ();
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
		Assert.Check (!Visitors.Contains (p), "Visitors doesn't contain the player that take over");
		Assert.Check (CanBeTakenOver, "Can't be taken over");

		this.Owner = p;
		this.Effect.ApplyEffect (p);
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
}
