using UnityEngine;


namespace PlayerStats
{
    public class PlayerStats : MonoBehaviour
    {
        public static PlayerStats Instance;

        [Header("Main Player Stats")]
        public string PlayerName;
        public int PlayerXP = 0;
        public int PlayerLevel = 1;


        [Header("Combat Stats")]
        public int damage;

        [Header("Health Stats")]
        public int maxHealth;
        public int currentHealth;

        [Header("Mana Stats")]
        public int maxMana;
        public int currentMana;

        void Start()
        {
            
        }

        private void Update()
        {
            
        }


    }
}
