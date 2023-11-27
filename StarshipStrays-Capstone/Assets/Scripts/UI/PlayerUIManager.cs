using PersistenceSystem;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] private Text _healthText;
    [SerializeField] private Text _coinText;

    public void UpdateUI(PlayerController player)
    {
        _coinText.text = player.Money.ToString();
        _healthText.text = player.currentHealth + " / " + player.maxHealth;
    }

    public void LoadData(GameData data)
    {
        _coinText.text = data.money.ToString();
    }

    public void SaveData(ref GameData data){}
}
