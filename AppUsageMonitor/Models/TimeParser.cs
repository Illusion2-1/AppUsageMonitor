namespace AppUsageMonitor.Models;

public class TimeParser {
    //takes in minutes and converts into 'hh:mm'
    public static string ConvertMinutesToTime(int minutes) {
        var hours = minutes / 60;
        var minutesRemainder = minutes % 60;
        var hoursString = hours.ToString();
        var minutesString = minutesRemainder.ToString();
        if (hours < 10) hoursString = "0" + hoursString;

        if (minutesRemainder < 10) minutesString = "0" + minutesString;
        return hoursString + ":" + minutesString;
    }
}