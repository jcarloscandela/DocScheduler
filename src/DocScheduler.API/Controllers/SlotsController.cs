using DocScheduler.Application;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace DocScheduler.API
{
    [Route("api/slots")]
    [ApiController]
    [Produces("application/json")]
    public class SlotsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public SlotsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService ?? throw new ArgumentNullException(nameof(appointmentService));
        }

        /// <summary>
        /// Retrieves available slots for the specified week starting from the provided Monday date.
        /// </summary>
        /// <param name="request">The Monday date of the week for which to retrieve available slots.</param>
        /// <returns>List of available slots.</returns>
        [HttpGet("available")]
        [ProducesResponseType(typeof(IEnumerable<AvailableSlotResponse>), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        public async Task<ActionResult<IEnumerable<AvailableSlotResponse>>> GetWeekAvailableSlots(
            [FromQuery][Required][SwaggerParameter("Date of the Monday for the week")] AvailableSlotRequest request)
        {
            var result = await _appointmentService.GetAvailableSlotsForWeekAsync(request);

            return Ok(result);
        }

        /// <summary>
        /// Attempts to reserve a slot based on the provided details.
        /// </summary>
        /// <param name="slotDetails">Details of the slot to be taken.</param>
        /// <returns>ActionResult indicating success or failure.</returns>
        [HttpPost("book")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> ReserveSlot(
            [FromBody][Required][SwaggerRequestBody("Details of the slot to be reserved")] BookSlotRequest request)
        {
            await _appointmentService.TakeSlotAsync(request);
            return Ok("Slot booked successfully.");
        }
    }
}