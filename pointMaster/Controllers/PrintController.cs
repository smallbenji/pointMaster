﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pointMaster.Data;
using pointMaster.Models;
using QRCoder;
using System.Drawing;

namespace pointMaster.Controllers
{
	public class PrintController : Controller
	{
		private readonly DataContext context;

		public PrintController(DataContext context)
		{
			this.context = context;
		}

		public async Task<IActionResult> Patruljer()
		{
			var vm = new PrintPatruljerModel();

			vm.patruljer = await context.Patruljer.Include(x => x.PatruljeMedlems).ToListAsync();

			foreach (var item in vm.patruljer)
			{
				QRCodeGenerator QrGenerator = new QRCodeGenerator();
				QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(Request.Host.Host + "/point/givpoint/" + item.Id, QRCodeGenerator.ECCLevel.Q);

				vm.QRcode.Add(item.Id, "data:image/png;base64," + Convert.ToBase64String(new PngByteQRCode(QrCodeInfo).GetGraphic(20)));
			}

			return View(vm);
		}

	}
		public class PrintPatruljerModel
		{
			public List<Patrulje> patruljer { get; set; } = null!;
			public Dictionary<int, string> QRcode { get; set; } = new Dictionary<int, string>();
		}
}
