﻿namespace WebAspDBeaverStudy.Interfaces
{
    public interface IImageWorker
    {
        string Save(string url);
        void Delete(string fileName);
    }
}
