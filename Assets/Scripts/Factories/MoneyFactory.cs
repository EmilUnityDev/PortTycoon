using UnityEngine;

public class MoneyFactory
{
    private const string MONEY_PATH = "Prefabs/VFX/MoneyParticle";

    public void CreateMoneyParticle(Vector3 worldPosition, int moneyAmount, Transform camera)
    {
        GameObject newParticle = Object.Instantiate(Resources.Load(MONEY_PATH)) as GameObject;
        newParticle.transform.position = worldPosition;
        newParticle.transform.rotation = camera.rotation;
        newParticle.GetComponent<MoneyParticle>().PlayParticle(moneyAmount);
    }
}