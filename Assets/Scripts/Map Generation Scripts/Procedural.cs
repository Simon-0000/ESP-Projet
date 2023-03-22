using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    public abstract class Procedural : MonoBehaviour
    {
        public abstract GameObject InstanciateProceduralObject(Transform parent);
    }
}
