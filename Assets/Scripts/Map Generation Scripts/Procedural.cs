//Auteurs: Simon Asmar
//Explication: Cette classe abstraite à pour but d'être implémentée par tous ceux qui peuvent être générés relativement à un parent
//Elle est utilisée par le script ProceduralHierarchy pour pouvoir générer des tuiles, des objets et d'autres hiérarchies
//de manière procédurale

using UnityEngine;

namespace Assets
{
    public abstract class Procedural : MonoBehaviour
    {
        public abstract GameObject InstanciateProcedural(Transform parent);
    }
}
