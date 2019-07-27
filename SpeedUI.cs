using UnityEngine;
using UnityEngine.UI;

namespace FLFlight.UI
{
    /// <summary>
    /// Shows throttle and speed of the player ship.
    /// </summary>
    public class SpeedUI : MonoBehaviour
    {
        private Text text;

        private void Awake()
        {
            text = GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            if (text != null && Ship.PlayerShip != null)
            {
                text.text = string.Format("{0}",
                                          Ship.PlayerShip.Velocity.magnitude.ToString("000"));
            }
        }
    }
}

