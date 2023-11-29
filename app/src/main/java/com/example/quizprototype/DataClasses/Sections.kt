package com.example.quizprototype.DataClasses

import com.example.quizprototype.DataClasses.Gk
import com.example.quizprototype.DataClasses.Movy
import com.example.quizprototype.DataClasses.Science

data class Sections(
    val gk: List<Gk>,
    val movies: List<Movy>,
    val science: List<Science>
)