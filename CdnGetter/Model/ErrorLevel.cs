namespace CdnGetter.Model
{
    public enum ErrorLevel
    {
        /// <summary>
        /// Logs that track the general flow of the application.
        /// </summary>
        Information = 0,
        
        /// <summary>
        /// Logs that highlight an abnormal or unexpected event in the application flow, but do not otherwise cause the application execution to stop.
        /// </summary>
        Warning = 1,
        
        /// <summary>
        /// Logs that highlight when the current flow of execution is stopped due to a failure.
        /// </summary>
        /// <remarks>These should indicate a failure in the current activity, not an application-wide failure.</remarks>
        Error = 2,
        
        /// <summary>
        /// Logs that describe an unrecoverable application or system crash, or a catastrophic failure that requires immediate attention.
        /// </summary>
        Critical = 3
    }
}