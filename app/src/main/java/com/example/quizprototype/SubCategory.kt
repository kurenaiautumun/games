package com.example.quizprototype

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import android.widget.Button
import android.widget.Toast
import com.google.firebase.FirebaseApp
import com.google.firebase.database.*

class SubCategory : AppCompatActivity() {
    private lateinit var cat1 :Button
    private lateinit var cat2 :Button
    private lateinit var cat3 :Button
    private lateinit var cat4 :Button
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_sub_category)

        FirebaseApp.initializeApp(this)

        cat1 = findViewById(R.id.cat1)
        cat2 = findViewById(R.id.cat2)
        cat3 = findViewById(R.id.cat3)
        cat4 = findViewById(R.id.cat4)
        val subcategories = mutableListOf<String>()

        val subsection = intent.getStringExtra("category")

        val database = FirebaseDatabase.getInstance()
        if(subsection=="SCI"){

            val scienceRef = database.getReference("question_bank/science")
            scienceRef.addListenerForSingleValueEvent(object : ValueEventListener {
                override fun onDataChange(dataSnapshot: DataSnapshot) {
                    val subcategories = mutableListOf<String>()

                    for (subcategorySnapshot in dataSnapshot.children) {
                        val subcategoryName = subcategorySnapshot.key
                        subcategoryName?.let {
                            subcategories.add(it)
//
                        }
                    }

                    for (i in 0 until minOf(subcategories.size, 4)) {
                        val currentCatButton = when (i) {
                            0 -> cat1
                            1 -> cat2
                            2 -> cat3
                            3 -> cat4
                            else -> null
                        }

                        currentCatButton?.let { button ->
                            val currentSubcategory = subcategories[i]
                            button.text = currentSubcategory
                            button.setOnClickListener {
                                clicking(subsection, currentSubcategory)
//                                Toast.makeText(this@SubCategory, currentSubcategory, Toast.LENGTH_LONG).show()
                            }
                        }
                    }


                }

                override fun onCancelled(databaseError: DatabaseError) {
                    // Handle errors
                    Log.e("Firebase", "Error: ${databaseError.message}")
                }
            })
        }

        else{
            val movieRef = database.getReference("question_bank/movies")
            movieRef.addListenerForSingleValueEvent(object : ValueEventListener {
                override fun onDataChange(dataSnapshot: DataSnapshot) {


                    for (subcategorySnapshot in dataSnapshot.children) {
                        val subcategoryName = subcategorySnapshot.key
                        subcategoryName?.let {
                            subcategories.add(it)
//
                        }
                    }

                    for (i in 0 until minOf(subcategories.size, 4)) {
                        val currentCatButton = when (i) {
                            0 -> cat1
                            1 -> cat2
                            2 -> cat3
                            3 -> cat4
                            else -> null
                        }

                        currentCatButton?.let { button ->
                            val currentSubcategory = subcategories[i]
                            button.text = currentSubcategory
                            button.setOnClickListener {
                                clicking(subsection, currentSubcategory)
//                                Toast.makeText(this@SubCategory, currentSubcategory, Toast.LENGTH_LONG).show()
                            }
                        }
                    }


                }

                override fun onCancelled(databaseError: DatabaseError) {
                    // Handle errors
                    Log.e("Firebase", "Error: ${databaseError.message}")
                }
            })

        }


    }

    private fun clicking(subsection: String?, subcategories: String) {

            val intent = Intent(this@SubCategory,Quizing::class.java)
            intent.putExtra("category",subsection)
            intent.putExtra("underSection",subcategories)
            startActivity(intent)
            finish()

    }
}