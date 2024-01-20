package com.example.quizprototype.DataClasses

data class modifiedquestions(
    var tries:Int,
    val section: String?,
    val question : String?,
    val option1 : String?,
    val option2 : String?,
    val option3 : String?,
    val Option4 : String?,
    val answer : String?
)
