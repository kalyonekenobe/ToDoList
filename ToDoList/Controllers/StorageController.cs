using Microsoft.AspNetCore.Mvc;
using ToDoList.Enums;

namespace ToDoList.Controllers
{
	public class StorageController : Controller
	{
		public static readonly Storages DefaultStorage = Storages.MsSql;

		[HttpGet]
		[Route("Storage/ChangeStorage/{storageId}/")]
		public IActionResult ChangeStorage(int storageId, string returnUri = "")
		{
			HttpContext.Response.Cookies.Append("StorageId", storageId.ToString());
			return Redirect(returnUri);
		}
	}
}
