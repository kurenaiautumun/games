package com.example.quizprototype

import android.content.Context
import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.os.CountDownTimer
import android.os.Handler
import android.os.Looper
import android.util.Log
import android.widget.*
import com.example.quizprototype.DataClasses.Questions
import com.example.quizprototype.DataClasses.modifiedquestions
import com.example.quizprototype.DataClasses.storedSubsection
import com.example.quizprototype.DataClasses.stored_modifiedquestions
import com.google.android.gms.ads.*
import com.google.android.gms.ads.interstitial.InterstitialAd
import com.google.android.gms.ads.interstitial.InterstitialAdLoadCallback
import com.google.firebase.FirebaseApp
import com.google.firebase.database.*
import com.google.gson.Gson
import com.google.gson.reflect.TypeToken
import java.util.*

class Quizing : AppCompatActivity() {

    private var mInterstitialAd: InterstitialAd? = null
    private lateinit var optionsRadioGroup : RadioGroup
    private lateinit var  submitButton : Button
    private lateinit var questionTextView: TextView
    private lateinit var option1RadioButton: RadioButton
    private lateinit var option2RadioButton: RadioButton
    private lateinit var option3RadioButton: RadioButton
    private lateinit var option4RadioButton: RadioButton
    private lateinit var result : TextView
    private lateinit var timerView : TextView
    private lateinit var questionList : MutableList<Questions>
    private lateinit var misslist : MutableList<modifiedquestions>
    private lateinit var Wronglist:MutableList<modifiedquestions>
    private lateinit var category: String
    private lateinit var subcategory : String
    private lateinit var storedModifiedquestionslist: MutableList<stored_modifiedquestions>
    private lateinit var newQuestionsListMiss : MutableList<modifiedquestions>
    private lateinit var newQuestionsListWrong : MutableList<modifiedquestions>
    private var questionFoundMiss = 0
    private var questionFoundWrong = 0

    private var currentQuestionIndex = 0
    private var score = 0
    private var timer: CountDownTimer? = null

    val totalCategories = 10 // Update this based on your actual requirement
    var completedCategories = 0

    private val handler = Handler(Looper.getMainLooper())
    private lateinit var mAdView : AdView

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_quizing)

        optionsRadioGroup = findViewById(R.id.optionsRadioGroup)
        submitButton = findViewById(R.id.submitButton)
        result = findViewById(R.id.resultTextView)
        questionTextView = findViewById(R.id.questionTextView)
        option1RadioButton = findViewById(R.id.optionARadioButton)
        option2RadioButton = findViewById(R.id.optionBRadioButton)
        option3RadioButton = findViewById(R.id.optionCRadioButton)
        option4RadioButton = findViewById(R.id.optionDRadioButton)
        timerView = findViewById(R.id.TimerView)

        questionList = mutableListOf()
        misslist = mutableListOf()
        Wronglist = mutableListOf()
        storedModifiedquestionslist = mutableListOf()

        MobileAds.initialize(this) {}
        FirebaseApp.initializeApp(this)

        mAdView = findViewById(R.id.adViewQuizing)
        val adRequestbanner = AdRequest.Builder().build()
        mAdView.loadAd(adRequestbanner)


        var adRequest = AdRequest.Builder().build()

        InterstitialAd.load(this,"ca-app-pub-3940256099942544/1033173712", adRequest, object : InterstitialAdLoadCallback() {
            override fun onAdFailedToLoad(adError: LoadAdError) {
                adError?.toString()?.let { Log.d("Activity", it) }
                mInterstitialAd = null
            }


            override fun onAdLoaded(interstitialAd: InterstitialAd) {
                Log.d("Activity", "Ad was loaded.")
                mInterstitialAd = interstitialAd
            }
        })

        category = intent.getStringExtra("category").toString()
        subcategory = intent.getStringExtra("underSection").toString()

        val sharedPreferences = getSharedPreferences("MyPrefs", Context.MODE_PRIVATE)
        val gson = Gson()
        val jsonW = sharedPreferences.getString("wrong", "")
        val jsonM = sharedPreferences.getString("miss", "")
        val storedListType = object : TypeToken<List<stored_modifiedquestions>>() {}.type
        var storedListWrong: List<stored_modifiedquestions> = gson.fromJson(jsonW, storedListType) ?: emptyList()
        var storedListMiss: List<stored_modifiedquestions> = gson.fromJson(jsonM, storedListType) ?: emptyList()

        val existingSectionIndexMiss = storedListMiss.indexOfFirst { it.title == category }
        val existingSectionIndexWrong = storedListWrong.indexOfFirst { it.title == category }
        newQuestionsListMiss = mutableListOf()
        newQuestionsListWrong = mutableListOf()

        val uniqueQuestionsSet = HashSet<String>()

        if(existingSectionIndexMiss!=-1){
            for (question in storedListMiss[existingSectionIndexMiss].subsections.toMutableList()) {
                if (uniqueQuestionsSet.add(question.question.toString())) {
                    // Only add if the question is not already in the set
                    newQuestionsListMiss.add(question)
                }
            }
        }
        if(existingSectionIndexWrong!=-1){
            for (question in storedListWrong[existingSectionIndexWrong].subsections.toMutableList()) {
                if (uniqueQuestionsSet.add(question.question.toString())) {
                    // Only add if the question is not already in the set
                    newQuestionsListWrong.add(question)
                }
            }
        }




        questionList = set_questions(category, subcategory!!)



        submitButton.setOnClickListener {
            processing_question(category)
        }

    }

    private fun display_Question() {

        result.text = ""
        val question = questionList[currentQuestionIndex]
        questionTextView.text = "Q.${currentQuestionIndex+1} "+ question.question
        option1RadioButton.text = question.option1
        option2RadioButton.text = question.option2
        option3RadioButton.text = question.option3
        option4RadioButton.text = question.Option4

        optionsRadioGroup.clearCheck()

        startTimer(11)
    }

    private fun set_questions(category: String?, subCategory: String): MutableList<Questions> {
        val questionList = mutableListOf<Questions>()
        val databaseReference = FirebaseDatabase.getInstance().reference
         val questionBankReference: DatabaseReference = databaseReference.child("question_bank")

        if(category=="Science"){
            val modifiedSubcategory = subCategory.lowercase(Locale.ROOT)
//            Toast.makeText(this,modifiedSubcategory,Toast.LENGTH_LONG).show()
            for (categoryIndex in 0 until 10){
                val categoryReference: DatabaseReference =
                    questionBankReference.child("science").child(modifiedSubcategory).child(categoryIndex.toString())

//                // Fetch data

                categoryReference.addListenerForSingleValueEvent(object : ValueEventListener {
                    override fun onDataChange(dataSnapshot: DataSnapshot) {

                        // Parse the dataSnapshot to get the required information
                        val question = dataSnapshot.child("question").getValue(String::class.java)
                        val answer = dataSnapshot.child("answer").getValue(String::class.java)
                        val options = dataSnapshot.child("options").children.map { it.getValue(String::class.java) }



                        questionList.add(Questions(categoryIndex,question,options[0],options[1],options[2],options[3],answer))

                        completedCategories++
                        if (completedCategories == totalCategories) {
                            // All categories are completed, notify the caller

                            onCategoryCompleted()
                        }
                    }

                    override fun onCancelled(databaseError: DatabaseError) {
                        // Handle errors
                        Log.e("Firebase", "Error fetching data", databaseError.toException())
                    }
                })
            }
        }

        else if(category=="Movies"){
            val modifiedSubcategory = subCategory.lowercase(Locale.ROOT)
            for (categoryIndex in 0 until 10) {
                val categoryReference: DatabaseReference =
                    questionBankReference.child("movies").child(modifiedSubcategory).child(categoryIndex.toString())

//                // Fetch data

                categoryReference.addListenerForSingleValueEvent(object : ValueEventListener {
                    override fun onDataChange(dataSnapshot: DataSnapshot) {

                        // Parse the dataSnapshot to get the required information
                        val question = dataSnapshot.child("question").getValue(String::class.java)
                        val answer = dataSnapshot.child("answer").getValue(String::class.java)
                        val options = dataSnapshot.child("options").children.map { it.getValue(String::class.java) }



                        questionList.add(Questions(categoryIndex,question,options[0],options[1],options[2],options[3],answer))

                        completedCategories++
                        if (completedCategories == totalCategories) {
                            // All categories are completed, notify the caller
                            onCategoryCompleted()
                        }
                    }

                    override fun onCancelled(databaseError: DatabaseError) {
                        // Handle errors
                        Log.e("Firebase", "Error fetching data", databaseError.toException())
                    }
                })
            }
        }

        else if(category=="GK") {


            // Reference to the "question_bank" node


            for (categoryIndex in 0 until 10) {
                val categoryReference: DatabaseReference =
                    questionBankReference.child("gk").child(categoryIndex.toString())

//                // Fetch data

                categoryReference.addListenerForSingleValueEvent(object : ValueEventListener {
                    override fun onDataChange(dataSnapshot: DataSnapshot) {

                        // Parse the dataSnapshot to get the required information
                        val question = dataSnapshot.child("question").getValue(String::class.java)
                        val answer = dataSnapshot.child("answer").getValue(String::class.java)
                        val options = dataSnapshot.child("options").children.map { it.getValue(String::class.java) }



                        questionList.add(Questions(categoryIndex,question,options[0],options[1],options[2],options[3],answer))

                        completedCategories++
                        if (completedCategories == totalCategories) {
                            // All categories are completed, notify the caller
                            onCategoryCompleted()
                        }
                    }

                    override fun onCancelled(databaseError: DatabaseError) {
                        // Handle errors
                        Log.e("Firebase", "Error fetching data", databaseError.toException())
                    }
                })
            }
        }

        Log.d("counting",questionList.size.toString())
        return questionList
    }

    private fun onCategoryCompleted() {
        // This function will be called once all categories are completed
        display_Question()
    }

    private fun processing_question(category: String?) {

            timer?.cancel()


//        Log.d("newlist",newQuestionsList.toString())

            val selectedRadioButtonId = optionsRadioGroup.checkedRadioButtonId
            currentQuestionIndex++
            val question_object = questionList[currentQuestionIndex-1]
            questionFoundMiss = newQuestionsListMiss.indexOfFirst{ it.question==question_object.question }
            questionFoundWrong = newQuestionsListWrong.indexOfFirst { it.question==question_object.question }
            if (selectedRadioButtonId != -1) {
                val selectedRadioButton = findViewById<RadioButton>(selectedRadioButtonId)
                val selectedAnswer = selectedRadioButton.text.toString()
                // Check the selected answer and perform further actions
                if(checkanswer(selectedAnswer)) {
                    result.text = "Correct Answer"
                    result.setTextColor(getColor(R.color.Correct))
                    score++



                    if(questionFoundMiss!=-1){
//                        Toast.makeText(this@Quizing,newQuestionsListMiss[questionFoundMiss].tries.toString(),Toast.LENGTH_LONG).show()
                        newQuestionsListMiss[questionFoundMiss].tries-=1
//                        Toast.makeText(this@Quizing,newQuestionsListMiss[questionFoundMiss].tries.toString(),Toast.LENGTH_LONG).show()

                    }
                    else if(questionFoundWrong!=-1){
                        newQuestionsListWrong[questionFoundWrong].tries-=1
                    }
                }
                else {
                    result.text = "Wrong Answer"
                    result.setTextColor(getColor(R.color.Incorrect))

//                    Log.d("test",question_object.question.toString())


                    if(questionFoundWrong!=-1){
                        newQuestionsListWrong[questionFoundWrong].tries+=1
                    }
                    else{
//                        Toast.makeText(this@Quizing,"hello",Toast.LENGTH_LONG).show()
                    Wronglist.add(modifiedquestions(3,category,question_object.question,question_object.option1,question_object.option2,question_object.option3,question_object.Option4,question_object.answer))
                    }
                }

            } else {
                // No answer selected

                if(questionFoundMiss!=-1){
//                    Toast.makeText(this@Quizing,"hel",Toast.LENGTH_LONG).show()
                    newQuestionsListMiss[questionFoundMiss].tries+=1
                }
                else{
                misslist.add(modifiedquestions(3,category,question_object.question,question_object.option1,question_object.option2,question_object.option3,question_object.Option4,question_object.answer))
                }
                result.text = "Wrong Answer"
                result.setTextColor(getColor(R.color.Incorrect))

            }






        if(currentQuestionIndex<questionList.size){

            if(questionFoundMiss!=-1)
                Log.d("question",newQuestionsListMiss.toString())
            handler.postDelayed({
                display_Question()
//                startTimer(10)
            }, 500)

        }
        else{
//            saveData(Wronglist,"wrong",newQuestionsListWrong)
//            Toast.makeText(this,newQuestionsListMiss.toString(),Toast.LENGTH_LONG).show()
//            Log.d("newlist1",newQuestionsListMiss.toString())
            saveData(misslist,"miss",newQuestionsListMiss)
            saveData(Wronglist,"wrong",newQuestionsListMiss)
//            Log.d("wrong",Wronglist.toString())
//            Log.d("miss",misslist.toString())
            if (mInterstitialAd != null) {
                showads()
            }
            else {
                val intent = Intent(this, Result::class.java)
                intent.putExtra("score", score.toString())
                startActivity(intent)
                finish()
            }
        }

    }
    fun saveData(list: MutableList<modifiedquestions>, saveString: String,changesQuestionlist: MutableList<modifiedquestions>) {

        val sharedPreferences = getSharedPreferences("MyPrefs", Context.MODE_PRIVATE)
        val editor = sharedPreferences.edit()

        // Retrieve existing data from SharedPreferences
        val gson = Gson()
        val existingJson = sharedPreferences.getString(saveString, "")
        val existingListType = object : TypeToken<List<stored_modifiedquestions>>() {}.type
        val existingList: MutableList<stored_modifiedquestions> =
            gson.fromJson(existingJson, existingListType) ?: mutableListOf()

        // Find the section in the existing list
        val existingSectionIndex = existingList.indexOfFirst { it.title == category }

        // Create new questions list
        val newQuestionsList = mutableListOf<modifiedquestions>()
        list.forEach {
            newQuestionsList.add(it)
        }


        if (existingSectionIndex != -1) {
//            Toast.makeText(this@Quizing,"hello",Toast.LENGTH_LONG).show()
            // If section exists, append new questions
            changesQuestionlist.removeIf { it.tries==0 }
            val existingSection = changesQuestionlist

            existingSection.addAll(newQuestionsList)
            existingList[existingSectionIndex] = stored_modifiedquestions(category, existingSection)
        } else {
            // If section does not exist, create a new entry
            existingList.add(stored_modifiedquestions(category, newQuestionsList))
        }

        // Save the updated list back to SharedPreferences
        val updatedJson = gson.toJson(existingList)
        editor.putString(saveString, updatedJson)
        editor.apply()
    }



    private fun showads() {
        mInterstitialAd?.fullScreenContentCallback = object : FullScreenContentCallback(){
            override fun onAdClicked() {
                super.onAdClicked()
            }

            override fun onAdDismissedFullScreenContent() {
                super.onAdDismissedFullScreenContent()
                val intent = Intent(this@Quizing,Result::class.java)
                intent.putExtra("score",score.toString())
                intent.putExtra("Quescount",questionList.size.toString())
                startActivity(intent)
                finish()
            }

            override fun onAdFailedToShowFullScreenContent(p0: AdError) {
                super.onAdFailedToShowFullScreenContent(p0)
            }

            override fun onAdImpression() {
                super.onAdImpression()
            }

            override fun onAdShowedFullScreenContent() {
                super.onAdShowedFullScreenContent()
            }

        }
        mInterstitialAd?.show(this)
    }

    private fun checkanswer(selectedAnswer: String): Boolean {
        val correctAnswer = questionList[currentQuestionIndex-1].answer

        return selectedAnswer == correctAnswer
    }

    private fun startTimer(seconds: Long) {
        timer = object : CountDownTimer(seconds * 1000, 1000) {
            override fun onTick(millisUntilFinished: Long) {
                // Update the timer display with the remaining time
                timerView.text = "Time remaining: ${millisUntilFinished / 1000} seconds"
            }

            override fun onFinish() {
                currentQuestionIndex++
                if (currentQuestionIndex < questionList.size) {
                    display_Question()
                }
                else{
                    if (mInterstitialAd != null) {
                        showads()
                    }
                    else {
                        val intent = Intent(this@Quizing, Result::class.java)
                        intent.putExtra("score", score.toString())
                        startActivity(intent)
                        finish()
                    }
                }

            }
        }.start()
    }

    override fun onDestroy() {
        // Cancel the timer when the activity is destroyed
        timer?.cancel()
        super.onDestroy()
    }
}