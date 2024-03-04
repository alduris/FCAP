using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCAP
{
    internal class GameController : UpdatableAndDeletable
    {
        public override void Update(bool eu)
        {
            base.Update(eu);
            Global.Update();
        }
    }
}
