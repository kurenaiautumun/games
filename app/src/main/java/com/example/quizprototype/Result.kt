package com.example.quizprototype

import android.content.Context
import android.content.Intent
import android.content.SharedPreferences
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.Button
import android.widget.TextView
import android.widget.Toast
import com.google.android.gms.ads.AdRequest
import com.google.android.gms.ads.AdView
import com.google.android.gms.ads.MobileAds

class Result : AppCompatActivity() {
    private lateinit var mAdView : AdView
    private lateinit var sharedPreferences: SharedPreferences
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_result)

        val missbutton = findViewById<Button>(R.id.miss)
        val wrongbutton = findViewById<Button>(R.id.wrong)
        missbutton.setOnClickListener {
            val intent = Intent(this,Post_Quiz::class.java)
            intent.putExtra("section","miss")
            startActivity(intent)
        }

        wrongbutton.setOnClickListener {
            val intent = Intent(this,Post_Quiz::class.java)
            intent.putExtra("section","wrong")
            startActivity(intent)
        }


        MobileAds.initialize(this) {}

        mAdView = findViewById(R.id.adViewResult)
        val adRequest = AdRequest.Builder().build()
        mAdView.loadAd(adRequest)

        val name = findViewById<TextView>(R.id.name)
        val score = findViewById<TextView>(R.id.score)

        sharedPreferences = getSharedPreferences("MyPrefs", Context.MODE_PRIVATE)

        val savedName = sharedPreferences.getString("name", "")


        name.text = "Name : "+savedName
        val scoring = intent.getStringExtra("score")
        score.text = "Score : "+scoring + "/10"
    }
}