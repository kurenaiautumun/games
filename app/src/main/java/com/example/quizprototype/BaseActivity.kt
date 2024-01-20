package com.example.quizprototype

import android.content.Context
import android.content.Intent
import android.content.SharedPreferences
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import android.view.Menu
import android.view.MenuItem
import android.widget.Toast
import androidx.appcompat.app.ActionBarDrawerToggle
import androidx.drawerlayout.widget.DrawerLayout
import androidx.fragment.app.Fragment
import com.example.quizprototype.DataClasses.User_Rank_Distribution
import com.example.quizprototype.Fragments.HomeFragment
import com.example.quizprototype.Fragments.ProfileFragment
import com.example.quizprototype.Fragments.missesFragment
import com.example.quizprototype.Fragments.wrongFragment
import com.example.quizprototype.Retrofit_Object_and_Interface.Retrofitobject
import com.google.android.material.bottomnavigation.BottomNavigationView
import com.google.android.material.navigation.NavigationView
import okhttp3.MultipartBody
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class BaseActivity : AppCompatActivity() {

    private lateinit var BottomNavigationView : BottomNavigationView
    private lateinit var sharedPreferences: SharedPreferences

    private lateinit var toggle: ActionBarDrawerToggle
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_base)


        val drawer_layout = findViewById<DrawerLayout>(R.id.drawable)
        val nav_view = findViewById<NavigationView>(R.id.Nav_View)

        toggle = ActionBarDrawerToggle(this,drawer_layout,R.string.open,R.string.close)


        drawer_layout.addDrawerListener(toggle)
        toggle.syncState()

        supportActionBar?.setDisplayHomeAsUpEnabled(true)

        nav_view.setNavigationItemSelectedListener {
            when(it.itemId){
                R.id.LiveQuiz-> startActivity(Intent(applicationContext,Active_Quiz::class.java))
                R.id.navranklist->startActivity(Intent(applicationContext,Quizlist::class.java))
                R.id.solution->startActivity(Intent(applicationContext,SolutionList::class.java))
                R.id.logout-> {
                    sharedPreferences = getSharedPreferences("MyPrefs", Context.MODE_PRIVATE)
                    val editor = sharedPreferences.edit()
                    editor.putString("UID","")
                    editor.putString("name","")
                    editor.putString("ph_number","")
                    editor.putString("quiz_name","")
                    editor.apply()
                    startActivity(Intent(applicationContext,Login::class.java))
                    finish()
                }
            }
            true
        }

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
                R.id.profile->{
                    replaceFragment(ProfileFragment())
                    true
                }
                else->{
                    false
                }
            }

        }
        replaceFragment(HomeFragment())
    }


    override fun onCreateOptionsMenu(menu: Menu?): Boolean {
        menuInflater.inflate(R.menu.navigation, menu)

        // Remove all menu items
        menu?.clear()

        return true
    }

    override fun onOptionsItemSelected(item: MenuItem): Boolean {
        if(toggle.onOptionsItemSelected(item)){
            return true
        }
        return super.onOptionsItemSelected(item)
    }

    private fun replaceFragment(fragment : Fragment){
        supportFragmentManager.beginTransaction().replace(R.id.FrameContainer,fragment).commit()
    }

}