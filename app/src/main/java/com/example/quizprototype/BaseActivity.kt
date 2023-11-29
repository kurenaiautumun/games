package com.example.quizprototype

import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import androidx.fragment.app.Fragment
import com.example.quizprototype.Fragments.HomeFragment
import com.example.quizprototype.Fragments.missesFragment
import com.example.quizprototype.Fragments.wrongFragment
import com.google.android.material.bottomnavigation.BottomNavigationView

class BaseActivity : AppCompatActivity() {

    private lateinit var BottomNavigationView : BottomNavigationView
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_base)

        BottomNavigationView = findViewById(R.id.bottomNavigation)

        BottomNavigationView.setOnItemSelectedListener{
            menuItem->
            when(menuItem.itemId){
                R.id.Bottom_home->{
                    replaceFragment(HomeFragment())
                    true
                }
                R.id.Bottom_misses->{
                    replaceFragment(missesFragment())
                    true
                }
                R.id.Bottom_wrong->{
                    replaceFragment(wrongFragment())
                    true
                }
                else->{
                    false
                }
            }

        }
        replaceFragment(HomeFragment())
    }

    private fun replaceFragment(fragment : Fragment){
        supportFragmentManager.beginTransaction().replace(R.id.FrameContainer,fragment).commit()
    }
}