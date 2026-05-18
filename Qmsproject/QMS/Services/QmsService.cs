using System;
using QMS.Models;

namespace QMS.Services;

public class QmsService
{
    public string Evaluate(Inspection inspection)
    {
        if (!inspection.Lens ||
            !inspection.Button ||
            !inspection.Power ||
            !inspection.Speaker)
        {
            return "Fail";
        }

        if (inspection.Temperature < 20 ||
            inspection.Temperature > 60)
        {
            return "Fail";
        }

        return "Pass";
    }
}
