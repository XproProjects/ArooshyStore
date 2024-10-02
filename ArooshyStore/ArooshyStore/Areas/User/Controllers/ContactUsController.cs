using ArooshyStore.BLL.EmailService;
using ArooshyStore.BLL.Interfaces;
using ArooshyStore.BLL.Services;
using ArooshyStore.Models.ViewModels;
using System.Linq;
using System.Web.Mvc;

namespace ArooshyStore.Areas.User.Controllers
{
    public class ContactUsController : Controller
    {
        private readonly ICompanyRepository _repository;


        public ContactUsController(ICompanyRepository repository)
        {
            _repository = repository;
        }
        public ActionResult Index()
        {
            var categories = _repository.GetFooterDataForCompany();
            return View(categories);
            // return View();

        }
        [HttpPost]
        public ActionResult SendMessage(ContactusViewModel contactForm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "Please fill in all required fields.";
                return View("Index");
            }

            System.Diagnostics.Debug.WriteLine($"Name: {contactForm.txtName}, Email: {contactForm.txtEmail}, Subject: {contactForm.txtSubject},PhoneNumber: {contactForm.txtPhone}, Message: {contactForm.txtMessage}");

            string subject = contactForm.txtSubject;
            string body = $"<h3>Contact Us</h3>" +
                          $"<strong>Name:</strong> {contactForm.txtName}<br/>" +
                          $"<strong>Email:</strong> {contactForm.txtEmail}<br/>" +
                           $"<strong>Subject:</strong> {subject}<br/>" +
                          $"<strong>PhoneNumber:</strong> {contactForm.txtPhone}<br/>" +
                          $"<strong>Message:</strong><br/> {contactForm.txtMessage}";

            var adminEmail = System.Configuration.ConfigurationManager.AppSettings["FromEmail"];
            var result = EmailServiceRepository.SendEmailString(adminEmail, subject, body);

            ViewBag.Message = result == "Success" ? "Your message has been sent successfully!" : "Failed to send message.";
            var categories = _repository.GetFooterDataForCompany();

            return View("Index", categories);
        }


    }
}

   