package com.example.quizprototype

import android.content.Context
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.os.CountDownTimer
import android.os.Handler
import android.os.Looper
import android.util.Log
import android.widget.Button
import android.widget.RadioButton
import android.widget.RadioGroup
import android.widget.TextView
import com.example.quizprototype.DataClasses.modifiedquestions
import com.google.gson.Gson
import com.google.gson.reflect.TypeToken

class Post_Quiz : AppCompatActivity() {

    private lateinit var optionsRadioGroup : RadioGroup
    private lateinit var  submitButton : Button
    private lateinit var questionTextView: TextView
    private lateinit var option1RadioButton: RadioButton
    private lateinit var option2RadioButton: RadioButton
    private lateinit var option3RadioButton: RadioButton
    private lateinit var option4RadioButton: RadioButton
    private lateinit var result : TextView
    private lateinit var timerView : TextView
    private lateinit var questionlist : MutableList<modifiedquestions>

    private var currentQuestionIndex = 0
    private var score = 0
    private var timer: CountDownTimer? = null

    val totalCategories = 0
    var completedCategories = 0

    private val handler = Handler(Looper.getMainLooper())

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_post_test)

        optionsRadioGroup = findViewById(R.id.optionsRadioGroupPOST)
        submitButton = findViewById(R.id.submitButtonPost)
        result = findViewById(R.id.resultTextViewPost)
        questionTextView = findViewById(R.id.questionTextViewPost)
        option1RadioButton = findViewById(R.id.optionARadioButtonPost)
        option2RadioButton = findViewById(R.id.optionBRadioButtonPost)
        option3RadioButton = findViewById(R.id.optionCRadioButtonPost)
        option4RadioButton = findViewById(R.id.optionDRadioButtonPost)
        timerView = findViewById(R.id.TimerViewPost)

        val sets = intent.getStringExtra("section")

        val sharedPreferences = getSharedPreferences("MyPrefs", Context.MODE_PRIVATE)
        val jsonString = sharedPreferences.getString(sets, "")

        // Convert JSON string back to List<ScienceData>
         questionlist = Gson().fromJson(jsonString, object : TypeToken<List<modifiedquestions>>() {}.type)

        Log.d("list",questionlist.toString())
    }
}