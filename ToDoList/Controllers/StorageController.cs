using Microsoft.AspNetCore.Mvc;

namespace ToDoList.Controllers
{
	public class StorageController : Controller
	{
		public static readonly Storages DefaultStorage = Storages.MsSql;

		public enum Storages
		{
			MsSql,
			Xml,
		}

		[HttpGet]
		[Route("Storage/ChangeStorage/{storageId}/")]
		public IActionResult ChangeStorage(int storageId, string returnUri = "")
		{
			HttpContext.Response.Cookies.Append("StorageId", storageId.ToString());
			return Redirect(returnUri);
		}
	}
}
