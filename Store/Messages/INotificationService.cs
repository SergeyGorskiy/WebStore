﻿using System.Threading.Tasks;

namespace Store
{
    public interface INotificationService
    {
        void SendConfirmationCode(string cellPhone, int code);

        Task SendConfirmationAsync(string cellPhone, int code);
        void StartProcess(Order order);
        Task StartProcessAsync(Order order);
    }
}