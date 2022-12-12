using System.Collections.Generic;
using UnityEngine;

public class CitiesGenerator : MonoBehaviour
{
    // Attributs
    [Header("Nombre d'habitants")]
    [SerializeField] int nombreMinHabitants;
    [SerializeField] int nombreMaxHabitants;
    [SerializeField] int nombreMinHabitantsVille;

    [Header("Caract�ristiques visuelles")]
    [SerializeField] float assetSizeInMeter;
    [SerializeField] private Transform cityParent;
    [SerializeField] City assetVille;
    [SerializeField] City assetVillage;
    
    [Header("Autre param�tres")]
    [SerializeField] float spawnRadius;
    [SerializeField] int nombreVilles;
    [SerializeField] float distanceFusion; // distance maximale d�terminant si on fusionne deux villes suffisamment proches entre elles
    [SerializeField] bool activateMerge; // activation de la fusion

    //public List<City> cities { get; } = new();
    public List<City> cities;
    private Transform parent;

    // Clusters contenant les villes fusionn�es & leurs indices
    private List<List<City>> _cluster;
    private List<int> _clusteredIndexes; 

    private void Awake()
    {
        parent = transform;
        cities = new ();
    }

    public void Generate()
    {
        GenerateCities(nombreVilles);
        if(activateMerge)
            MergeAllCities(cities);

        // Adapte la hauteur de la ville au terrain
        foreach (var city in cities)
        {
            var ray = new Ray(city.transform.position + new Vector3(0f, 500f, 0f), Vector3.down);
            if (Physics.Raycast(ray, out var hit))
            {
                city.transform.position = hit.point;
            }
        }
    }

    // G�n�rateur de ville
    private void GenerateCities(int _nombreVilles)
    {
        for(int i = 0; i < _nombreVilles; i++)
        {
            string nomVille;
            City.CityType type;
            City asset;
            // G�n�ration al�atoire du nombre d'habitants
            int nombreHabitants = Random.Range(nombreMinHabitants, nombreMaxHabitants);
            // Adaptation de certains caract�ristiques selon le type de ville, d�termin� par le nombre d'habitants
            if (nombreHabitants < nombreMinHabitantsVille)
            {
                nomVille = "Village n�" + i;
                asset = assetVillage;
            }
            else
            {
                    nomVille = "Ville n�" + i;
                    asset = assetVille;
            }
            cities.Add(GenerateCityObject(asset, GetRandomPosition(), nomVille, nombreHabitants));
        }
    }

    // Fusion de toutes les villes de la map, pendant une it�ration
    private void MergeAllCities(List<City> baseCities)
    {
        // Pr�parations
        List<City> newCities = new List<City>();
        int mergedCitiesNumber = 0;
        int currentClusterIndex = 0;
        _cluster = new List<List<City>>();
        _clusteredIndexes = new List<int>();
        // R�partitions des villes dans les clusters
        for (int i = 0; i < baseCities.Count - 1; i++)
        {
            // On passe si la ville est d�j� incluse dans un cluster
            if (_clusteredIndexes.Contains(i))
                continue;
            
            // Cr�ation de la portion de cluster & ajout de la ville 
            _cluster.Add(new List<City>());
            _cluster[currentClusterIndex].Add(baseCities[i]);
            _clusteredIndexes.Add(i);
            // V�rification des distances avec les autres villes
            Vector3 positionA = baseCities[i].transform.position;
            for (int j = i + 1; j < baseCities.Count; j++)
            {
                // On passe si la ville est d�j� incluse dans un cluster
                if (_clusteredIndexes.Contains(j))
                    continue;
                
                Vector3 positionB = baseCities[j].transform.position;
                if (Vector3.Distance(positionA, positionB) <= distanceFusion)
                {
                    _cluster[currentClusterIndex].Add(baseCities[j]);
                    _clusteredIndexes.Add(j);
                }    
            }
            currentClusterIndex++;
        }
        // Fusion des villes issues du m�me cluster
        for(int k = 0; k < _cluster.Count; k++)
        {
            if (_cluster[k].Count == 0)
                continue;
            
            if (_cluster[k].Count == 1)
                newCities.Add(_cluster[k][0]);
            else
                newCities.Add(MergeCities(_cluster[k], k));
        }
        // Mise � jour de la liste de villes
        DestroyAllCities(cities, newCities);
        cities.Clear();
        cities = newCities;
    }

    // Fusion entre plusieurs villes, issues d'une liste
    private City MergeCities(List<City> cities, int index)
    {
        // Pr�paration
        City biggerCity;
        City biggerCityAsset;
        Vector3 sommePositions = Vector3.zero;
        Vector3 finalPosition = new Vector3();
        int finalHabitantsNumber = 0;
        string finalName;
        // Parcours des villes � fusionner
        foreach (var city in cities)
        {
            sommePositions += city.transform.position;
            if (city.GetComponent<City>() != null)
            {
                finalHabitantsNumber += city.GetComponent<City>().NombreHabitants;
                Debug.Log("City trouv�e");
            }
            else
                Debug.Log("City introuvable");
        }
        finalPosition = sommePositions / cities.Count;
        // Adaptation de certains param�tres
        if (finalHabitantsNumber < nombreMinHabitantsVille)
        {
            Debug.Log("Village fusionn� de " + finalHabitantsNumber);
            finalName = "Village fusionn� n�" + index;
            biggerCityAsset = assetVillage;
        }
        else
        {
            finalName = "Ville fusionn�e n�" + index;
            biggerCityAsset = assetVille;
        }
        
        // Param�trage de la nouvelle ville
        biggerCity = GenerateCityObject(biggerCityAsset, finalPosition, finalName, finalHabitantsNumber);
        return biggerCity;
    }

    // Destruction des villes si elle ne sont pas conserv�es lors de la fusion
    private void DestroyAllCities(List<City> cities, List<City> savedCities)
    {
        foreach(var city in cities)
        {
            if(!savedCities.Contains(city))
                Destroy(city);
        }
    }

    // Cr�ation d'un GameObject repr�sentant une ville
    private City GenerateCityObject(City asset, Vector3 newPosition, string newName, int nombreHabitants)
    {
        var cityObject = Instantiate(asset, newPosition, Quaternion.identity, cityParent);
        cityObject.name = newName;
        cityObject.NombreHabitants = nombreHabitants;
        //cityObject.transform.localScale = new Vector3(assetSizeInMeter, assetSizeInMeter, assetSizeInMeter);
        return cityObject;
    }

    // G�n�rateur al�atoire de position
    private Vector3 GetRandomPosition()
    {
        Vector2 randPos = Random.insideUnitCircle * spawnRadius;
        return parent.position + new Vector3(randPos.x, 0f, randPos.y);
    }
}
