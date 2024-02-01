﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeApp.Models
{
    public enum GameState
    {
        WaitingToStart,
        WaitingForUserInput,
        InProgress,
        EndGame
    }
}
