using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    //------------------------------------------------------------
    // Variables
    //------------------------------------------------------------
    private string bannerName = "Banner_player_";

    private string medalSubFolder = "Art";
    private string bronzeMedalName = "END_MEDAL_Bronze";
    private string silverMedalName = "END_MEDAL_Silver";
    private string goldMedalName = "END_MEDAL_Gold";

    //------------------------------------------------------------

    void OnEnable ()
	{
		GameObject[] banner = new GameObject[4];

		for (int i = 0; i < 4; i++)
		{
			string instanceBannerName = bannerName + (i + 1).ToString();
			banner[i] = this.transform.FindChild(instanceBannerName).gameObject;
			banner[i].transform.FindChild("bronzeMedalPos").GetComponent<Image>().enabled = false;
			banner[i].transform.FindChild("silverMedalPos").GetComponent<Image>().enabled = false;
			banner[i].transform.FindChild("goldMedalPos")	.GetComponent<Image>().enabled = false;
			banner[i].transform.FindChild("Cache")	.GetComponent<Image>().enabled = true;
		}

        for (int i = 0; i < Game.Instance.Players.Length; i++)
        {
            Player player = Game.Instance.Players[i];

			InstantiateMedal(player.nbrRoundWin, banner[(int)player.controller - 1]);
			banner[(int)player.controller - 1].transform.FindChild("Crown").GetComponent<Image>().enabled = (player.nbrRoundWin == 3);

            if(player.nbrRoundWin == 3)
            {
                Debug.Log("Player in color : " + player.colorZones.ToString() + " has won the Game ! #YOLOSWAAG");
				banner[(int)player.controller - 1].GetComponent<Image>().enabled = true;
            }
        }
        //------------------------------------------------------------
    }

    //Instantiate The medal GameObjects
    void InstantiateMedal(int nbMedal, GameObject banner)
	{
		banner.transform.FindChild("Cache")	.GetComponent<Image>().enabled = false;
        switch (nbMedal)
        {
            case 0:
                break;
            case 1:
                banner.transform.FindChild("bronzeMedalPos").GetComponent<Image>().enabled = true;
                break;
            case 2:
                banner.transform.FindChild("bronzeMedalPos").GetComponent<Image>().enabled = true;
                banner.transform.FindChild("silverMedalPos").GetComponent<Image>().enabled = true;
                break;
            case 3:
                banner.transform.FindChild("bronzeMedalPos").GetComponent<Image>().enabled = true;
                banner.transform.FindChild("silverMedalPos").GetComponent<Image>().enabled = true;
                banner.transform.FindChild("goldMedalPos")	.GetComponent<Image>().enabled = true;
                break;
            default:
                break;
        }
    }
}
