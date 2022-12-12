using UnityEngine;

public class City : MonoBehaviour
{
    // Attributs
    public Vector3 Position => transform.position;
    public CityType Type => _type;
    
    public int NombreHabitants;
    [SerializeField] private CityType _type;

    // Constructeurs
    public City(Vector3 position, int nombreHabitants, CityType type, GameObject cityAsset, float size, string name)
    {
        transform.position = position;
        NombreHabitants = nombreHabitants;
        _type = type;
        //this.DisplayCity(cityAsset, size, name);
    }

    public City(Vector3 position, int nombreHabitants)
    {
        transform.position = position;
        NombreHabitants = nombreHabitants;
    }

    // Affichage de la ville
    //void DisplayCity(GameObject cityAsset, float size, string name)
    //{
    //    GameObject cityDisplay = Instantiate(cityAsset, this.Position, Quaternion.identity);
    //    cityDisplay.transform.localScale = new Vector3(size, 2 * size, size);
    //    cityDisplay.name = name;
    //}

    // Type de ville
    public enum CityType
    {
        VILLE,
        VILLAGE
    }
}
