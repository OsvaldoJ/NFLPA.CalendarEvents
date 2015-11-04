using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Orchard.CalendarEvents.Models;
using Orchard.CalendarEvents.ViewModels;

namespace Orchard.CalendarEvents.Formatters
{
    public class iCalFormatResult : ContentResult
    {
        private readonly CalendarViewModel _calendarView;
        private readonly EventIcs _eventIcs;
        private string _contentType;
        //private string _calendarResponseAsString;

        public iCalFormatResult(string contentType, object calendar)
           // : base(contentType)
        {
            _contentType = contentType;
            _calendarView = calendar as CalendarViewModel;
            if (_calendarView == null)
            {
                _eventIcs = calendar as EventIcs;
                if (_eventIcs == null)
                {
                    throw new InvalidOperationException("Cannot serialize type");
                }
            }

        }

        public string getResponseAsString()
        {
            var streamWriter = new StringBuilder();

            if (_calendarView != null)
            {
                WriteToStringBuilder(_calendarView.Events, streamWriter);
            }
            else if (_eventIcs != null)
            {
                streamWriter.AppendLine("BEGIN:VCALENDAR");
                streamWriter.AppendLine("PRODID:-//NFL Players Association//Calendar 1.0//EN");
                streamWriter.AppendLine("VERSION:2.0");
                streamWriter.AppendLine("CALSCALE:GREGORIAN");
                streamWriter.AppendLine("X-WR-TIMEZONE:" + _eventIcs.TimeZone);
                streamWriter.AppendLine("TZ:+00");
                WriteAppointmentToStringBuilder(_eventIcs, streamWriter);
                streamWriter.AppendLine("END:VCALENDAR");
            }

            return streamWriter.ToString();
        }

        public void WriteToStringBuilder(object value, StringBuilder writer)
        {
            writer.AppendLine("BEGIN:VCALENDAR");
            writer.AppendLine("PRODID:-//NFL Players Association//Calendar 1.0//EN");
            writer.AppendLine("VERSION:2.0");
            writer.AppendLine("CALSCALE:GREGORIAN");
            writer.AppendLine("METHOD:PUBLISH");
            writer.AppendLine("REFRESH-INTERVAL;VALUE=DURATION:PT2H");
            writer.AppendLine("X-PUBLISHED-TTL:PT2H");
            writer.AppendLine("X-WR-CALNAME:" + _calendarView.Title);
            writer.AppendLine("NAME:" + _calendarView.Title);
            writer.AppendLine("X-WR-CALDESC:" + _calendarView.Summary);
            writer.AppendLine("X-WR-TIMEZONE:America/New_York");
            if (!string.IsNullOrWhiteSpace(_calendarView.CategoriesCsv))
                writer.AppendLine("CATEGORIES:" + _calendarView.CategoriesCsv);
            var appointments = value as IEnumerable<EventIcs>;
            if (appointments != null)
            {
                foreach (var appointment in appointments)
                {
                    WriteAppointmentToStringBuilder(appointment, writer);
                }
            }
            else
            {
                var singleAppointment = value as EventIcs;
                if (singleAppointment == null)
                {
                    throw new InvalidOperationException("Cannot serialize type");
                }
                WriteAppointmentToStringBuilder(singleAppointment, writer);
            }
            writer.AppendLine("END:VCALENDAR");
        }

        public void WriteAppointmentToStringBuilder(EventIcs evt, StringBuilder writer)
        {

            writer.AppendLine("BEGIN:VEVENT");
            writer.AppendLine("UID:" + evt.Id + "@nflpa.com");
            if (evt.IsAllDay)
            {
                writer.AppendLine("DTSTART;VALUE=DATE:" +
                    string.Format("{0:yyyyMMdd}", evt.Start));
                writer.AppendLine("DTEND;VALUE=DATE:" +
                    string.Format("{0:yyyyMMdd}", evt.End));
            }
            else
            {
                writer.AppendLine("DTSTART;TZID=" + evt.TimeZone + ":" +
                    string.Format("{0:yyyyMMddTHHmmssZ}", evt.Start));
                writer.AppendLine("DTEND;TZID=" + evt.TimeZone + ":" +
                    string.Format("{0:yyyyMMddTHHmmssZ}", evt.End));
            }
            writer.AppendLine("SUMMARY:" + evt.Title);
            writer.AppendLine("DESCRIPTION:" + evt.Summary);
            //if (!string.IsNullOrWhiteSpace(evt.LocationName))
            writer.AppendLine("LOCATION:" + evt.LocationName);
            if (!string.IsNullOrWhiteSpace(evt.CategoriesCsv))
                writer.AppendLine("CATEGORIES:" + evt.CategoriesCsv);
            writer.AppendLine("DTSTAMP:" + string.Format("{0:yyyyMMddTHHmmssZ}", evt.ModifiedDateTime));
            writer.AppendLine("CREATED:" + string.Format("{0:yyyyMMddTHHmmssZ}", evt.CreateDateTime));
            writer.AppendLine("URL:" + evt.Url);
            writer.AppendLine("END:VEVENT");
        }
        public void WriteToStream(object value, StreamWriter writer)
        {
            writer.WriteLine("BEGIN:VCALENDAR");
            writer.WriteLine("PRODID:-//NFL Players Association//Calendar 1.0//EN");
            writer.WriteLine("VERSION:2.0");
            writer.WriteLine("CALSCALE:GREGORIAN");
            writer.WriteLine("METHOD:PUBLISH");
            writer.WriteLine("REFRESH-INTERVAL;VALUE=DURATION:PT2H");
            writer.WriteLine("X-PUBLISHED-TTL:PT2H");
            writer.WriteLine("X-WR-CALNAME:" + _calendarView.Title);
            writer.WriteLine("NAME:" + _calendarView.Title);
            writer.WriteLine("X-WR-CALDESC:" + _calendarView.Summary);
            writer.WriteLine("X-WR-TIMEZONE:America/New_York");
            if (!string.IsNullOrWhiteSpace(_calendarView.CategoriesCsv))
                writer.WriteLine("CATEGORIES:" + _calendarView.CategoriesCsv);
            var appointments = value as IEnumerable<EventIcs>;
            if (appointments != null)
            {
                foreach (var appointment in appointments)
                {
                    WriteAppointment(appointment, writer);
                }
            }
            else
            {
                var singleAppointment = value as EventIcs;
                if (singleAppointment == null)
                {
                    throw new InvalidOperationException("Cannot serialize type");
                }
                WriteAppointment(singleAppointment, writer);
            }
            writer.WriteLine("END:VCALENDAR");
        }

        public void WriteAppointment(EventIcs evt, StreamWriter writer)
        {

            writer.WriteLine("BEGIN:VEVENT");
            writer.WriteLine("UID:" + evt.Id + "@nflpa.com");
            if (evt.IsAllDay)
            {
                writer.WriteLine("DTSTART;VALUE=DATE:" + 
                    string.Format("{0:yyyyMMdd}", evt.Start));
                writer.WriteLine("DTEND;VALUE=DATE:" + 
                    string.Format("{0:yyyyMMdd}", evt.End));
            }
            else
            {
                writer.WriteLine("DTSTART;TZID=" + evt.TimeZone + ":" + 
                    string.Format("{0:yyyyMMddTHHmmssZ}", evt.Start));
                writer.WriteLine("DTEND;TZID=" + evt.TimeZone + ":" + 
                    string.Format("{0:yyyyMMddTHHmmssZ}", evt.End));
            }
            writer.WriteLine("SUMMARY:" + evt.Title);
            writer.WriteLine("DESCRIPTION:" + evt.Summary);
            //if (!string.IsNullOrWhiteSpace(evt.LocationName))
                writer.WriteLine("LOCATION:" + evt.LocationName);
            if (!string.IsNullOrWhiteSpace(evt.CategoriesCsv))
                writer.WriteLine("CATEGORIES:" + evt.CategoriesCsv);
            writer.WriteLine("DTSTAMP:" + string.Format("{0:yyyyMMddTHHmmssZ}", evt.ModifiedDateTime));
            writer.WriteLine("CREATED:" + string.Format("{0:yyyyMMddTHHmmssZ}", evt.CreateDateTime));
            writer.WriteLine("URL:" + evt.Url);
            writer.WriteLine("END:VEVENT");
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            response.Clear();
            response.ClearContent();
            response.BufferOutput = true;
            response.ContentType = _contentType;
            var streamWriter = new StreamWriter(response.OutputStream, Encoding.UTF8);
            //write the header line first
            if (_calendarView != null)
            {
                WriteToStream(_calendarView.Events, streamWriter);
            }
            else if (_eventIcs != null)
            {
                streamWriter.WriteLine("BEGIN:VCALENDAR");
                streamWriter.WriteLine("PRODID:-//NFL Players Association//Calendar 1.0//EN");
                streamWriter.WriteLine("VERSION:2.0");
                streamWriter.WriteLine("CALSCALE:GREGORIAN");
                streamWriter.WriteLine("X-WR-TIMEZONE:" + _eventIcs.TimeZone);
                streamWriter.WriteLine("TZ:+00");
                WriteAppointment(_eventIcs, streamWriter);
                streamWriter.WriteLine("END:VCALENDAR");
            }

            streamWriter.Flush();
            streamWriter.Close();

            base.ExecuteResult(context);
        }
        /*
        protected override void WriteFile(HttpResponseBase response)
        {
            response.Clear();
            response.ClearContent();
            response.BufferOutput = false;
            var streamWriter = new StreamWriter(response.OutputStream, Encoding.UTF8);
            //write the header line first
            if (_calendarView != null)
            {
                WriteToStream(_calendarView.Events, streamWriter);
            }
            else if (_eventIcs != null)
            {
                streamWriter.WriteLine("BEGIN:VCALENDAR");
                streamWriter.WriteLine("PRODID:-//NFL Players Association//Calendar 1.0//EN");
                streamWriter.WriteLine("VERSION:2.0");
                streamWriter.WriteLine("CALSCALE:GREGORIAN");
                streamWriter.WriteLine("X-WR-TIMEZONE:"+_eventIcs.TimeZone);
                streamWriter.WriteLine("TZ:+00");
                WriteAppointment(_eventIcs, streamWriter);
                streamWriter.WriteLine("END:VCALENDAR");
            }

            streamWriter.Flush();
            streamWriter.Close();
        }
        */
    }
}