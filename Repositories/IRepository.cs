using System.Linq;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories
{
	interface IRepository<T>
	{
		/// <summary>
		/// Get's all the items of type T
		/// </summary>
		/// <returns>All the items</returns>
		IQueryable<T> FindAll();
		/// <summary>
		/// Gets all the active items of type T
		/// </summary>
		/// <returns></returns>
		IQueryable<T> FindAllActive();
		/// <summary>
		/// Gets an item by its key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		T Get(int key);
		/// <summary>
		/// gets an item by its unique string identifier
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		T Get(string id);
	}
}
