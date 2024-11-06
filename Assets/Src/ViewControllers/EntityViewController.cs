using Game.Ids;
using UnityEngine;

namespace Game.ViewControllers
{
	/// <summary>
	/// Simple Mono Component containing the object's <seealso cref="UniqueId"/>
	/// </summary>
	public class EntityViewController : MonoBehaviour
	{
		/// <summary>
		/// This entity's <seealso cref="UniqueId"/>
		/// </summary>
		public UniqueId UniqueId;
	}
}