//Auteurs: Simon Asmar
//Explication: Cette classe abstraite à pour but d'être implémenter par tous ceux qui être générer par rapport à un parent
//Elle est utilisé par le script ProceduralHierarchy pour pouvoir générer des tuiles, des objets et d'autres hierarchies
//de manière procédural

using UnityEngine;

namespace Assets
{
    public abstract class Procedural : MonoBehaviour
    {
        public abstract GameObject InstanciateProcedural(Transform parent);
    }
}
